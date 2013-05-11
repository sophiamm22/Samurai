﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Model;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Exceptions;
using Samurai.Domain.Entities;
using Samurai.Core;
using Samurai.Domain.HtmlElements;

namespace Samurai.Domain.Value.Async
{
  public interface IAsyncCouponStrategy
  {
    Task<IEnumerable<IGenericTournamentCoupon>> GetTournaments(OddsDownloadStage stage = OddsDownloadStage.Tournament);
    Task<IEnumerable<GenericMatchCoupon>> GetMatches(Uri tournamentURL);
    Task<IEnumerable<GenericMatchCoupon>> GetMatches();
  }

  public abstract class AbstractAsyncCouponStrategy : IAsyncCouponStrategy
  {
    protected readonly IBookmakerRepository bookmakerRepository;
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly IWebRepositoryProviderAsync webRepositoryProvider;
    protected readonly IValueOptions valueOptions;
    protected List<MissingTeamPlayerAlias> missingAlias;

    public AbstractAsyncCouponStrategy(IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepositoryProviderAsync webRepositoryProvider, 
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

    public abstract Task<IEnumerable<IGenericTournamentCoupon>> GetTournaments(OddsDownloadStage stage = OddsDownloadStage.Tournament);
    public abstract Task<IEnumerable<GenericMatchCoupon>> GetMatches(Uri tournamentURL);

    public async Task<IEnumerable<GenericMatchCoupon>> GetMatches()
    {
      var couponURL = 
        this.bookmakerRepository
            .GetTournamentCouponUrl(this.valueOptions.Tournament, this.valueOptions.OddsSource);

      if (couponURL == null)
        throw new ArgumentNullException("couponURL");

      return await GetMatches(couponURL);
    }

    protected bool CheckPlayers(TeamPlayer teamOrPlayerA, TeamPlayer teamOrPlayerB, 
      string teamOrPlayerALookup, string teamOrPlayerBLookup)
    {
      bool @continue = false;
      if (teamOrPlayerA == null)
      {
        this.missingAlias.Add(new MissingTeamPlayerAlias
        {
          TeamOrPlayerName = teamOrPlayerALookup,
          ExternalSource = this.valueOptions.OddsSource.Source,
          Tournament = this.valueOptions.Tournament.TournamentName
        });
        @continue = true;
      }
      if (teamOrPlayerB == null)
      {
        this.missingAlias.Add(new MissingTeamPlayerAlias
        {
          TeamOrPlayerName = teamOrPlayerBLookup,
          ExternalSource = this.valueOptions.OddsSource.Source,
          Tournament = this.valueOptions.Tournament.TournamentName
        });
        @continue = true;
      }
      return @continue;
    }

  }

  public class BestBettingAsyncCouponStrategy<TCompetition> : AbstractAsyncCouponStrategy
    where TCompetition : IBestBettingCompetition, new()
  {
    public BestBettingAsyncCouponStrategy(IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepositoryProviderAsync webRepositoryProvider,
      IValueOptions valueOptions)
      : base(bookmakerRepository, fixtureRepository, webRepositoryProvider, valueOptions)
    { }

    public override async Task<IEnumerable<IGenericTournamentCoupon>> GetTournaments(OddsDownloadStage stage = OddsDownloadStage.Tournament)
    {
      var competitionsReturn = new List<IGenericTournamentCoupon>();

      var webRepository = 
        this.webRepositoryProvider
            .CreateWebRepository(this.valueOptions.CouponDate);

      var html = await webRepository.GetHTML(this.bookmakerRepository.GetTournamentCouponUrl(this.valueOptions.Tournament, this.valueOptions.OddsSource),
        string.Format("{0} BestBetting Coupon", this.valueOptions.CouponDate.ToShortDateString()));

      var bestbettingCompetitions =
        WebUtils.ParseWebsite<TCompetition>(html, s => { })
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
          matchesToAdd.AddRange(await GetMatches(t.CompetitionURL));
          tournament.Matches = matchesToAdd;
        }
        competitionsReturn.Add(tournament);
      }
      return competitionsReturn;
    }

    public override async Task<IEnumerable<GenericMatchCoupon>> GetMatches(Uri competitionURL)
    {
      var returnMatches = new List<GenericMatchCoupon>();

      var webRepository = 
        this.webRepositoryProvider
            .CreateWebRepository(this.valueOptions.CouponDate);

      var html = await webRepository.GetHTML(competitionURL);

      var matchTokens = 
        WebUtils.ParseWebsite<BestBettingScheduleDate, BestBettingScheduleMatch, BestBettingScheduleInRunning>(html, s => { })
                .ToList();

      var currentDate = DateTime.Now.Date;
      var lastChecked = DateTime.Now;

      var valSam = 
        this.fixtureRepository
            .GetExternalSource("Value Samurai");

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

          var teamOrPlayerA = 
            this.fixtureRepository
                .GetAlias(match.TeamOrPlayerA, this.valueOptions.OddsSource, valSam, this.valueOptions.Sport);
          var teamOrPlayerB = 
            this.fixtureRepository
                .GetAlias(match.TeamOrPlayerB, this.valueOptions.OddsSource, valSam, this.valueOptions.Sport);

          if (CheckPlayers(teamOrPlayerA, teamOrPlayerB, match.TeamOrPlayerA, match.TeamOrPlayerB))
            continue;

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
          if (returnMatches.Count != 0)
            returnMatches.Last().InPlay = true;
        }
      }
      if (this.missingAlias.Count > 0)
        throw new MissingTeamPlayerAliasException(this.missingAlias, "Missing team or player alias");

      return returnMatches;
    }
  }

  public class OddsCheckerMobiAsyncCouponStrategy<TCompetition> : AbstractAsyncCouponStrategy
    where TCompetition : IOddsCheckerCompetition, new()
  {
    public OddsCheckerMobiAsyncCouponStrategy(IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepositoryProviderAsync webRepositoryProvider,
      IValueOptions valueOptions)
      : base(bookmakerRepository, fixtureRepository, webRepositoryProvider, valueOptions)
    { }

    public override async Task<IEnumerable<IGenericTournamentCoupon>> GetTournaments(OddsDownloadStage stage = OddsDownloadStage.Tournament)
    {
      var tournamentsReturn = new List<IGenericTournamentCoupon>();

      var webRepository = 
        this.webRepositoryProvider
            .CreateWebRepository(this.valueOptions.CouponDate);

      var html = await webRepository.GetHTML(this.bookmakerRepository.GetTournamentCouponUrl(this.valueOptions.Tournament, this.valueOptions.OddsSource));

      var oddscheckerCompetitions =
        WebUtils.ParseWebsite<TCompetition>(html, s => { })
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
          matchesToAdd.AddRange(await GetMatches(t.CompetitionURL));
          competetion.Matches = matchesToAdd;
        }
        tournamentsReturn.Add(competetion);
      }
      return tournamentsReturn;
    }

    public override async Task<IEnumerable<GenericMatchCoupon>> GetMatches(Uri competitionURL)
    {
      var returnMatches = new List<GenericMatchCoupon>();

      var webRepository = 
        this.webRepositoryProvider
            .CreateWebRepository(this.valueOptions.CouponDate);

      var html = await webRepository.GetHTML(competitionURL);


      var matchTokens =
        WebUtils.ParseWebsite<OddsCheckerMobiGenericMatch, OddsCheckerMobiScheduleHeading>(html, s => { })
                .ToList();

      var lastChecked = DateTime.Now;

      var valSam = 
        this.fixtureRepository
            .GetExternalSource("Value Samurai");

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

  public class OddsCheckerWebAsyncCouponStrategy<TCompetition> : OddsCheckerMobiAsyncCouponStrategy<TCompetition>
    where TCompetition : IOddsCheckerCompetition, new()
  {
    public OddsCheckerWebAsyncCouponStrategy(IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepositoryProviderAsync webRepositoryProvider,
      IValueOptions valueOptions)
      : base(bookmakerRepository, fixtureRepository, webRepositoryProvider, valueOptions)
    { }

    public override async Task<IEnumerable<GenericMatchCoupon>> GetMatches(Uri competitionURL)
    {
      var returnMatches = new List<GenericMatchCoupon>();

      var webRepository = 
        this.webRepositoryProvider
            .CreateWebRepository(this.valueOptions.CouponDate);

      var html = await webRepository.GetHTML(competitionURL);

      var matchTokens =
        WebUtils.ParseWebsite<OddsCheckerWebScheduleDate, OddsCheckerWebScheduleMatch, OddsCheckerWebScheduleHeading>(html, s => { });

      var currentDate = DateTime.Now.Date;
      var lastChecked = DateTime.Now;

      var valSam = 
        this.fixtureRepository
            .GetExternalSource("Value Samurai");
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

          var teamOrPlayerA = 
            this.fixtureRepository
                .GetAlias(match.TeamOrPlayerA, this.valueOptions.OddsSource, valSam, this.valueOptions.Sport);
          var teamOrPlayerB = 
            this.fixtureRepository
                .GetAlias(match.TeamOrPlayerB, this.valueOptions.OddsSource, valSam, this.valueOptions.Sport);

          if (CheckPlayers(teamOrPlayerA, teamOrPlayerB, match.TeamOrPlayerA, match.TeamOrPlayerB)) 
            continue;

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
