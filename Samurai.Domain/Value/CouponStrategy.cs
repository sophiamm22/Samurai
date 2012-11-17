using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Domain.Model;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Core;
using Samurai.Domain.HtmlElements;

namespace Samurai.Domain.Value
{
  public interface ICouponStrategy
  {
    IEnumerable<IGenericTournamentCoupon> GetTournaments(OddsDownloadStage stage = OddsDownloadStage.Tournament);
    IEnumerable<IGenericMatchCoupon> GetMatches(Uri tournamentURL);
    IEnumerable<IGenericMatchCoupon> GetMatches();
  }

  public abstract class AbstractCouponStrategy : ICouponStrategy
  {
    protected readonly IBookmakerRepository bookmakerRepository;
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly IWebRepository webRepository;
    protected readonly IValueOptions valueOptions;

    public AbstractCouponStrategy(IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepository webRepository, 
      IValueOptions valueOptions)
    {
      if (bookmakerRepository == null) throw new ArgumentNullException("bookmakerRepository");
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (webRepository == null) throw new ArgumentNullException("webRepository");
      if (valueOptions == null) throw new ArgumentNullException("valueOptions");

      this.bookmakerRepository = bookmakerRepository;
      this.fixtureRepository = fixtureRepository;
      this.webRepository = webRepository;
      this.valueOptions = valueOptions;

    }

    public abstract IEnumerable<IGenericTournamentCoupon> GetTournaments(OddsDownloadStage stage = OddsDownloadStage.Tournament);
    public abstract IEnumerable<IGenericMatchCoupon> GetMatches(Uri tournamentURL);

    public IEnumerable<IGenericMatchCoupon> GetMatches()
    {
      return GetMatches(this.bookmakerRepository.GetTournamentCouponUrl(this.valueOptions.Tournament, this.valueOptions.OddsSource));
    }
  }

  public class BestBettingCouponStrategy<TCompetition> : AbstractCouponStrategy
    where TCompetition : IBestBettingCompetition, new()
  {
    public BestBettingCouponStrategy(IBookmakerRepository bookmakerRepository, 
      IFixtureRepository fixtureRepository, IWebRepository webRepository,
      IValueOptions valueOptions)
      : base(bookmakerRepository, fixtureRepository, webRepository, valueOptions)
    { }

    public override IEnumerable<IGenericTournamentCoupon> GetTournaments(OddsDownloadStage stage = OddsDownloadStage.Tournament)
    {
      var competitionsReturn = new List<IGenericTournamentCoupon>();

      var html = this.webRepository.GetHTML(new[] { this.bookmakerRepository.GetTournamentCouponUrl(this.valueOptions.Tournament, this.valueOptions.OddsSource) },
        s => Console.WriteLine(s), string.Format("{0} BestBetting Coupon", this.valueOptions.CouponDate.ToShortDateString()))
        .First();

      var bestbettingCompetitions = WebUtils.ParseWebsite<TCompetition>(html, s => Console.WriteLine(s))
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
          var matchesToAdd = new List<IGenericMatchCoupon>();
          matchesToAdd.AddRange(GetMatches(t.CompetitionURL));
          tournament.Matches = matchesToAdd;
        }
        competitionsReturn.Add(tournament);
      }
      return competitionsReturn;
    }

    public override IEnumerable<IGenericMatchCoupon> GetMatches(Uri competitionURL)
    {
      var returnMatches = new List<IGenericMatchCoupon>();

      var html = this.webRepository.GetHTML(new[] { competitionURL }, s => Console.WriteLine(s), competitionURL.ToString())
                               .First();

      var matchTokens = WebUtils.ParseWebsite<BestBettingScheduleDate, BestBettingScheduleMatch>(html,
        s => Console.WriteLine(s)).ToList();

      var currentDate = DateTime.Now.Date;

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

          var teamOrPlayerA = this.fixtureRepository.GetAlias(match.TeamOrPlayerA, this.valueOptions.OddsSource, this.fixtureRepository.GetExternalSource("Value Samurai"));
          var teamOrPlayerB = this.fixtureRepository.GetAlias(match.TeamOrPlayerB, this.valueOptions.OddsSource, this.fixtureRepository.GetExternalSource("Value Samurai"));

          var matchData = new GenericMatchCoupon
          {
            MatchURL = match.MatchURL,
            TeamOrPlayerA = teamOrPlayerA,
            TeamOrPlayerB = teamOrPlayerB,
            MatchDate = currentDate.AddHours(double.Parse(matchTime[0])).AddHours(double.Parse(matchTime[1]) / 60.0),
            Source = this.valueOptions.OddsSource.Source,
            LastChecked = DateTime.Now
          };

          matchData.HeadlineOdds = match.BestOdds;

          returnMatches.Add(matchData);
        }
      }
      return returnMatches;
    }
  }

  public class OddsCheckerMobiCouponStrategy<TCompetition> : AbstractCouponStrategy
    where TCompetition : IOddsCheckerCompetition, new()
  {
    public OddsCheckerMobiCouponStrategy(IBookmakerRepository bookmakerRepository, 
      IFixtureRepository fixtureRepository, IWebRepository webRepository,
      IValueOptions valueOptions)
      : base(bookmakerRepository, fixtureRepository, webRepository, valueOptions)
    { }

    public override IEnumerable<IGenericTournamentCoupon> GetTournaments(OddsDownloadStage stage = OddsDownloadStage.Tournament)
    {
      var tournamentsReturn = new List<IGenericTournamentCoupon>();

      var html = this.webRepository.GetHTML(new[] { this.bookmakerRepository.GetTournamentCouponUrl(this.valueOptions.Tournament, this.valueOptions.OddsSource) },
        s => Console.WriteLine(s), string.Format("{0} OddsChecker Coupon", this.valueOptions.CouponDate.ToShortDateString()))
        .First();

      var oddscheckerCompetitions = WebUtils.ParseWebsite<TCompetition>(html, s => Console.WriteLine(s))
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
          var matchesToAdd = new List<IGenericMatchCoupon>();
          matchesToAdd.AddRange(GetMatches(t.CompetitionURL));
          competetion.Matches = matchesToAdd;
        }
        tournamentsReturn.Add(competetion);
      }
      return tournamentsReturn;
    }

    public override IEnumerable<IGenericMatchCoupon> GetMatches(Uri competitionURL)
    {
      var returnMatches = new List<IGenericMatchCoupon>();

      var html = this.webRepository.GetHTML(new[] { competitionURL }, s => Console.WriteLine(s), competitionURL.ToString())
                               .First();

      var matchTokens = WebUtils.ParseWebsite<OddsCheckerMobiGenericMatch>(html, s => Console.WriteLine(s))
        .Cast<OddsCheckerMobiGenericMatch>()
        .ToList();

      foreach (var match in matchTokens)
      {
        var teamOrPlayerA = this.fixtureRepository.GetAlias(match.TeamOrPlayerA, this.valueOptions.OddsSource, this.fixtureRepository.GetExternalSource("Value Samurai"));
        var teamOrPlayerB = this.fixtureRepository.GetAlias(match.TeamOrPlayerB, this.valueOptions.OddsSource, this.fixtureRepository.GetExternalSource("Value Samurai"));

        var matchData = new GenericMatchCoupon
        {
          MatchURL = match.MatchURL,
          TeamOrPlayerA = teamOrPlayerA,
          TeamOrPlayerB = teamOrPlayerB,
          Source = this.valueOptions.OddsSource.Source,
          LastChecked = DateTime.Now
        };
        returnMatches.Add(matchData);
      }
      return returnMatches;
    }
  }

  public class OddsCheckerWebCouponStrategy<TCompetition> : OddsCheckerMobiCouponStrategy<TCompetition>
    where TCompetition : IOddsCheckerCompetition, new()
  {
    public OddsCheckerWebCouponStrategy(IBookmakerRepository bookmakerRepository, 
      IFixtureRepository fixtureRepository, IWebRepository webRepository,
      IValueOptions valueOptions)
      : base(bookmakerRepository, fixtureRepository, webRepository, valueOptions)
    { }

    public override IEnumerable<IGenericMatchCoupon> GetMatches(Uri competitionURL)
    {
      var returnMatches = new List<IGenericMatchCoupon>();

      var html = this.webRepository.GetHTML(new[] { competitionURL }, s => Console.WriteLine(s), competitionURL.ToString())
                               .First();
      var matchTokens = WebUtils.ParseWebsite<OddsCheckerWebScheduleDate, OddsCheckerWebScheduleMatch>(html,
        s => Console.WriteLine(s));

      var currentDate = DateTime.Now.Date;

      foreach (var token in matchTokens)
      {
        if (token is OddsCheckerWebScheduleDate)
        {
          currentDate = ((OddsCheckerWebScheduleDate)token).ScheduleDate;
        }
        else if (token is OddsCheckerWebScheduleMatch)
        {
          var match = ((OddsCheckerWebScheduleMatch)token);
          var matchTime = match.TimeString.Split(':');

          var teamOrPlayerA = this.fixtureRepository.GetAlias(match.TeamOrPlayerA, this.valueOptions.OddsSource, this.fixtureRepository.GetExternalSource("Value Samurai"));
          var teamOrPlayerB = this.fixtureRepository.GetAlias(match.TeamOrPlayerB, this.valueOptions.OddsSource, this.fixtureRepository.GetExternalSource("Value Samurai"));

          var matchData = new GenericMatchCoupon
          {
            MatchURL = match.MatchURL,
            TeamOrPlayerA = teamOrPlayerA,
            TeamOrPlayerB = teamOrPlayerB,
            MatchDate = currentDate.AddHours(double.Parse(matchTime[0])).AddHours(double.Parse(matchTime[1]) / 60.0),
            Source = this.valueOptions.OddsSource.Source,
            LastChecked = DateTime.Now
          };

          matchData.HeadlineOdds = match.BestOdds;

          returnMatches.Add(matchData);

        }
      }
      return returnMatches;
    }
  }
}
