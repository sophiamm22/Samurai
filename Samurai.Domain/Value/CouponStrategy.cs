using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Domain.Model;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Core;
using Samurai.Domain.HtmlElements;
using Samurai.Domain.Exceptions;
using Samurai.Domain.Entities;
using Samurai.Domain.Infrastructure;

namespace Samurai.Domain.Value
{
  public interface ICouponStrategy
  {
    IEnumerable<IGenericTournamentCoupon> GetTournaments(OddsDownloadStage stage = OddsDownloadStage.Tournament);
    IEnumerable<GenericMatchCoupon> GetMatches(Uri tournamentURL);
    IEnumerable<GenericMatchCoupon> GetMatches();
  }

  public abstract class AbstractCouponStrategy : ICouponStrategy
  {
    protected readonly IBookmakerRepository bookmakerRepository;
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly IWebRepositoryProvider webRepositoryProvider;
    protected readonly IValueOptions valueOptions;
    protected List<MissingTeamPlayerAlias> missingAlias;

    public AbstractCouponStrategy(IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepositoryProvider webRepositoryProvider, 
      IValueOptions valueOptions)
    {
      if (bookmakerRepository == null) throw new ArgumentNullException("bookmakerRepository");
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (webRepositoryProvider == null) throw new ArgumentNullException("webRepository");
      if (valueOptions == null) throw new ArgumentNullException("valueOptions");

      this.bookmakerRepository = bookmakerRepository;
      this.fixtureRepository = fixtureRepository;
      this.webRepositoryProvider = webRepositoryProvider;
      this.valueOptions = valueOptions;

      this.missingAlias = new List<MissingTeamPlayerAlias>();
    }

    public abstract IEnumerable<IGenericTournamentCoupon> GetTournaments(OddsDownloadStage stage = OddsDownloadStage.Tournament);
    public abstract IEnumerable<GenericMatchCoupon> GetMatches(Uri tournamentURL);

    public IEnumerable<GenericMatchCoupon> GetMatches()
    {
      var couponURL = this.bookmakerRepository.GetTournamentCouponUrl(this.valueOptions.Tournament, this.valueOptions.OddsSource);
      if (couponURL == null)
        throw new ArgumentNullException("couponURL");
      
      return GetMatches(couponURL);
    }

    protected bool CheckPlayers(TeamPlayer teamOrPlayerA, TeamPlayer teamOrPlayerB, string teamOrPlayerALookup, string teamOrPlayerBLookup)
    {
      bool @continue = false;
      if (teamOrPlayerA == null)
      {
        this.missingAlias.Add(new MissingTeamPlayerAlias { TeamOrPlayerName = teamOrPlayerALookup, ExternalSource = this.valueOptions.OddsSource.Source, Tournament = this.valueOptions.Tournament.TournamentName });
        @continue = true;
      }
      if (teamOrPlayerB == null)
      {
        this.missingAlias.Add(new MissingTeamPlayerAlias { TeamOrPlayerName = teamOrPlayerBLookup, ExternalSource = this.valueOptions.OddsSource.Source, Tournament = this.valueOptions.Tournament.TournamentName });
        @continue = true;
      }
      return @continue;
    }

  }

  public class BestBettingCouponStrategy<TCompetition> : AbstractCouponStrategy
    where TCompetition : IBestBettingCompetition, new()
  {
    public BestBettingCouponStrategy(IBookmakerRepository bookmakerRepository, 
      IFixtureRepository fixtureRepository, IWebRepositoryProvider webRepositoryProvider,
      IValueOptions valueOptions)
      : base(bookmakerRepository, fixtureRepository, webRepositoryProvider, valueOptions)
    { }

    public override IEnumerable<IGenericTournamentCoupon> GetTournaments(OddsDownloadStage stage = OddsDownloadStage.Tournament)
    {
      var competitionsReturn = new List<IGenericTournamentCoupon>();

      var webRepository = this.webRepositoryProvider.CreateWebRepository(this.valueOptions.CouponDate);

      var html = webRepository.GetHTML(new[] { this.bookmakerRepository.GetTournamentCouponUrl(this.valueOptions.Tournament, this.valueOptions.OddsSource) },
        s => ProgressReporterProvider.Current.ReportProgress(s, ReporterImportance.Medium), 
        string.Format("{0} BestBetting Coupon", this.valueOptions.CouponDate.ToShortDateString()))
        .First();

      var bestbettingCompetitions = WebUtils.ParseWebsite<TCompetition>(html, s => ProgressReporterProvider.Current.ReportProgress(s, ReporterImportance.Medium))
        .Cast<TCompetition>()
        .Where(c => c.CompetitionType == this.valueOptions.Tournament.TournamentName)
        .ToList();

      foreach (var t in bestbettingCompetitions)
      {
        if (competitionsReturn.Count(tMain => tMain.TournamentName == t.CompetitionName) != 0)
          continue; //guard against repetition on BestBetting tennis

        var tournament = new GenericTournamentCoupon
        {
          TournamentName = t.CompetitionName,
          TournamentURL = t.CompetitionURL,
        };

        if (stage != OddsDownloadStage.Tournament)
        {
          var matchesToAdd = new List<GenericMatchCoupon>();
          matchesToAdd.AddRange(GetMatches(t.CompetitionURL));
          tournament.Matches = matchesToAdd;
        }
        competitionsReturn.Add(tournament);
      }
      return competitionsReturn;
    }

    public override IEnumerable<GenericMatchCoupon> GetMatches(Uri competitionURL)
    {
      var returnMatches = new List<GenericMatchCoupon>();

      var webRepository = this.webRepositoryProvider.CreateWebRepository(this.valueOptions.CouponDate);

      var html = webRepository.GetHTML(new[] { competitionURL }, s => ProgressReporterProvider.Current.ReportProgress(s, ReporterImportance.Medium), competitionURL.ToString())
                              .First();

      var matchTokens = WebUtils.ParseWebsite<BestBettingScheduleDate, BestBettingScheduleMatch, BestBettingScheduleInRunning>(html,
        s => ProgressReporterProvider.Current.ReportProgress(s, ReporterImportance.Medium)).ToList();

      var currentDate = DateTime.Now.Date;
      var lastChecked = DateTime.Now;

      var valSam = this.fixtureRepository.GetExternalSource("Value Samurai");

      foreach (var token in matchTokens)
      {
        if (token is BestBettingScheduleDate)
        {
          currentDate = ((BestBettingScheduleDate)token).ScheduleDate;
        }
        else if (token is BestBettingScheduleMatch)
        {
          var match = ((BestBettingScheduleMatch)token);
          var matchTime = match.TimeString.Split(':');

          var teamOrPlayerA = this.fixtureRepository.GetAlias(match.TeamOrPlayerA, this.valueOptions.OddsSource, valSam, this.valueOptions.Sport);
          var teamOrPlayerB = this.fixtureRepository.GetAlias(match.TeamOrPlayerB, this.valueOptions.OddsSource, valSam, this.valueOptions.Sport);

          if (CheckPlayers(teamOrPlayerA, teamOrPlayerB, match.TeamOrPlayerA, match.TeamOrPlayerB)) continue;

          var matchData = new GenericMatchCoupon
          {
            MatchURL = match.MatchURL,
            TeamOrPlayerA = teamOrPlayerA.Name,
            FirstNameA = teamOrPlayerA.FirstName,
            TeamOrPlayerB = teamOrPlayerB.Name,
            FirstNameB = teamOrPlayerB.FirstName,
            MatchDate = currentDate.AddHours(double.Parse(matchTime[0])).AddHours(double.Parse(matchTime[1]) / 60.0),
            Source = this.valueOptions.OddsSource.Source,
            LastChecked = lastChecked
          };

          matchData.HeadlineOdds = match.BestOdds;

          returnMatches.Add(matchData);
        }
        else if (token is BestBettingScheduleInRunning)
        {
          returnMatches.Last().InPlay = true;
        }
      }
      if (this.missingAlias.Count > 0)
        throw new MissingTeamPlayerAliasException(this.missingAlias, "Missing team or player alias");

      return returnMatches;
    }
  }

  public class OddsCheckerMobiCouponStrategy<TCompetition> : AbstractCouponStrategy
    where TCompetition : IOddsCheckerCompetition, new()
  {
    public OddsCheckerMobiCouponStrategy(IBookmakerRepository bookmakerRepository, 
      IFixtureRepository fixtureRepository, IWebRepositoryProvider webRepositoryProvider,
      IValueOptions valueOptions)
      : base(bookmakerRepository, fixtureRepository, webRepositoryProvider, valueOptions)
    { }

    public override IEnumerable<IGenericTournamentCoupon> GetTournaments(OddsDownloadStage stage = OddsDownloadStage.Tournament)
    {
      var tournamentsReturn = new List<IGenericTournamentCoupon>();

      var webRepository = this.webRepositoryProvider.CreateWebRepository(this.valueOptions.CouponDate);

      var html = webRepository.GetHTML(new[] { this.bookmakerRepository.GetTournamentCouponUrl(this.valueOptions.Tournament, this.valueOptions.OddsSource) },
        s => ProgressReporterProvider.Current.ReportProgress(s, ReporterImportance.Medium), string.Format("{0} OddsChecker Coupon", this.valueOptions.CouponDate.ToShortDateString()))
        .First();

      var oddscheckerCompetitions = WebUtils.ParseWebsite<TCompetition>(html, s => ProgressReporterProvider.Current.ReportProgress(s, ReporterImportance.Medium))
        .Cast<TCompetition>()
        .Where(c => c.CompetitionType == this.valueOptions.Tournament.TournamentName)
        .ToList();

      foreach (var t in oddscheckerCompetitions)
      {
        var competetion = new GenericTournamentCoupon()
        {
          TournamentName = t.CompetitionName,
          TournamentURL = t.CompetitionURL,
        };

        if (stage != OddsDownloadStage.Tournament)
        {
          var matchesToAdd = new List<GenericMatchCoupon>();
          matchesToAdd.AddRange(GetMatches(t.CompetitionURL));
          competetion.Matches = matchesToAdd;
        }
        tournamentsReturn.Add(competetion);
      }
      return tournamentsReturn;
    }

    public override IEnumerable<GenericMatchCoupon> GetMatches(Uri competitionURL)
    {
      var returnMatches = new List<GenericMatchCoupon>();

      var webRepository = this.webRepositoryProvider.CreateWebRepository(this.valueOptions.CouponDate);

      var html = webRepository.GetHTML(new[] { competitionURL }, s => ProgressReporterProvider.Current.ReportProgress(s, ReporterImportance.Medium), competitionURL.ToString())
                              .First();

      var matchTokens = WebUtils.ParseWebsite<OddsCheckerMobiGenericMatch, OddsCheckerMobiScheduleHeading>(html, s => ProgressReporterProvider.Current.ReportProgress(s, ReporterImportance.Medium))
        .ToList();

      var lastChecked = DateTime.Now;

      var valSam = this.fixtureRepository.GetExternalSource("Value Samurai");

      var currentHeading = string.Empty;
      foreach (var token in matchTokens)
      {
        if (token is OddsCheckerMobiScheduleHeading)
        {
          currentHeading = ((OddsCheckerMobiScheduleHeading)token).Heading;
        }
        else if (token is OddsCheckerMobiGenericMatch && currentHeading == "Matches")
        {
          var match = (OddsCheckerMobiGenericMatch)token;
          var teamOrPlayerA = this.fixtureRepository.GetAlias(match.TeamOrPlayerA, this.valueOptions.OddsSource, valSam, this.valueOptions.Sport);
          var teamOrPlayerB = this.fixtureRepository.GetAlias(match.TeamOrPlayerB, this.valueOptions.OddsSource, valSam, this.valueOptions.Sport);

          if (CheckPlayers(teamOrPlayerA, teamOrPlayerB, match.TeamOrPlayerA, match.TeamOrPlayerB)) continue;

          var matchData = new GenericMatchCoupon
          {
            MatchURL = match.MatchURL,
            TeamOrPlayerA = teamOrPlayerA.Name,
            FirstNameA = teamOrPlayerA.FirstName,
            TeamOrPlayerB = teamOrPlayerB.Name,
            FirstNameB = teamOrPlayerB.FirstName,
            Source = this.valueOptions.OddsSource.Source,
            LastChecked = lastChecked,
            InPlay = match.InPlay
          };
          returnMatches.Add(matchData);
        }
        if (this.missingAlias.Count > 0)
          throw new MissingTeamPlayerAliasException(this.missingAlias, "Missing team or player alias");
      }
      return returnMatches;
    }
  }

  public class OddsCheckerWebCouponStrategy<TCompetition> : OddsCheckerMobiCouponStrategy<TCompetition>
    where TCompetition : IOddsCheckerCompetition, new()
  {
    public OddsCheckerWebCouponStrategy(IBookmakerRepository bookmakerRepository, 
      IFixtureRepository fixtureRepository, IWebRepositoryProvider webRepositoryProvider,
      IValueOptions valueOptions)
      : base(bookmakerRepository, fixtureRepository, webRepositoryProvider, valueOptions)
    { }

    public override IEnumerable<GenericMatchCoupon> GetMatches(Uri competitionURL)
    {
      var returnMatches = new List<GenericMatchCoupon>();

      var webRepository = this.webRepositoryProvider.CreateWebRepository(this.valueOptions.CouponDate);

      var html = webRepository.GetHTML(new[] { competitionURL }, s => ProgressReporterProvider.Current.ReportProgress(s, ReporterImportance.Medium), competitionURL.ToString())
                              .First();
      var matchTokens = WebUtils.ParseWebsite<OddsCheckerWebScheduleDate, OddsCheckerWebScheduleMatch, OddsCheckerWebScheduleHeading>(html,
        s => ProgressReporterProvider.Current.ReportProgress(s, ReporterImportance.Medium));

      var currentDate = DateTime.Now.Date;
      var lastChecked = DateTime.Now;

      var valSam = this.fixtureRepository.GetExternalSource("Value Samurai");
      var firstHeading = "Not set"; 

      foreach (var token in matchTokens)
      {
        if (token is OddsCheckerWebScheduleDate)
        {
          currentDate = ((OddsCheckerWebScheduleDate)token).ScheduleDate;
        }
        else if (token is OddsCheckerWebScheduleHeading)
        {
          firstHeading = firstHeading == "Not set" ? "1st heading" : "Not 1st heading";
        }
        else if (token is OddsCheckerWebScheduleMatch && firstHeading == "1st heading")
        {
          var match = ((OddsCheckerWebScheduleMatch)token);
          var matchTime = match.TimeString.Split(':');

          if (match.TeamOrPlayerA.IndexOf('/') > -1 || match.TeamOrPlayerB.IndexOf('/') > -1)
            continue;

          var teamOrPlayerA = this.fixtureRepository.GetAlias(match.TeamOrPlayerA, this.valueOptions.OddsSource, valSam, this.valueOptions.Sport);
          var teamOrPlayerB = this.fixtureRepository.GetAlias(match.TeamOrPlayerB, this.valueOptions.OddsSource, valSam, this.valueOptions.Sport);

          if (CheckPlayers(teamOrPlayerA, teamOrPlayerB, match.TeamOrPlayerA, match.TeamOrPlayerB)) continue;

          var matchData = new GenericMatchCoupon
          {
            MatchURL = match.MatchURL,
            TeamOrPlayerA = teamOrPlayerA.Name,
            FirstNameA = teamOrPlayerA.FirstName,
            TeamOrPlayerB = teamOrPlayerB.Name,
            FirstNameB = teamOrPlayerB.FirstName,
            MatchDate = currentDate.AddHours(double.Parse(matchTime[0])).AddHours(double.Parse(matchTime[1]) / 60.0),
            Source = this.valueOptions.OddsSource.Source,
            LastChecked = lastChecked,
            InPlay = match.InPlay
          };

          matchData.HeadlineOdds = match.BestOdds;

          returnMatches.Add(matchData);

        }
      }
      if (this.missingAlias.Count > 0)
        throw new MissingTeamPlayerAliasException(this.missingAlias, "Missing team or player alias");

      return returnMatches;
    }
  }
}
