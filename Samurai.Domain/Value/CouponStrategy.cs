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
  public abstract class AbstractCouponStrategy
  {
    protected readonly IBookmakerRepository bookmakerService;
    protected readonly IWebRepository webRepository;
    protected readonly IValueOptions valueOptions;

    public AbstractCouponStrategy(IBookmakerRepository bookmakerService,
      IWebRepository webRepository, IValueOptions valueOptions)
    {
      this.bookmakerService = bookmakerService;
      this.webRepository = webRepository;
      this.valueOptions = valueOptions;
    }

    public abstract IEnumerable<IGenericCompetitionCoupon> GetCompetitions(OddsDownloadStage stage = OddsDownloadStage.Competition);
    public abstract IEnumerable<IGenericMatchCoupon> GetMatches(Uri competitionURL);
  }

  public class BestBettingCouponStrategy<TCompetition> : AbstractCouponStrategy
    where TCompetition : IBestBettingCompetition, new()
  {
    public BestBettingCouponStrategy(IBookmakerRepository bookmakerService, IWebRepository webRepository,
      IValueOptions valueOptions)
      : base(bookmakerService, webRepository, valueOptions)
    { }

    public override IEnumerable<IGenericCompetitionCoupon> GetCompetitions(OddsDownloadStage stage = OddsDownloadStage.Competition)
    {
      var competitionsReturn = new List<IGenericCompetitionCoupon>();

      var html = this.webRepository.GetHTML(new[] { this.bookmakerService.GetCompetitionCouponUrl(this.valueOptions.Competition, this.valueOptions.OddsSource) },
        s => Console.WriteLine(s), string.Format("{0} BestBetting Coupon", this.valueOptions.CouponDate.ToShortDateString()))
        .First();

      var bestbettingCompetitions = WebUtils.ParseWebsite<TCompetition>(html, s => Console.WriteLine(s))
        .Cast<TCompetition>()
        .Where(c => c.CompetitionType == this.valueOptions.Competition.CompetitionName)
        .ToList();

      foreach (var t in bestbettingCompetitions)
      {
        if (competitionsReturn.Count(tMain => tMain.CompetitionName == t.CompetitionName) != 0)
          continue; //guard against repetition on BestBetting tennis

        var tournament = new GenericCompetitionCoupon
        {
          CompetitionName = t.CompetitionName,
          CompetitionURL = t.CompetitionURL,
        };

        if (stage != OddsDownloadStage.Competition)
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

          var obj = new TCompetition();
          var teamOrPlayerA = obj.ConvertTeamOrPlayerName(match.TeamOrPlayerA);
          var teamOrPlayerB = obj.ConvertTeamOrPlayerName(match.TeamOrPlayerB);

          var matchData = new GenericMatchCoupon
          {
            MatchURL = match.MatchURL,
            TeamOrPlayerA = teamOrPlayerA,
            TeamOrPlayerB = teamOrPlayerB,
            MatchDate = currentDate.AddHours(double.Parse(matchTime[0])).AddHours(double.Parse(matchTime[1]) / 60.0),
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
    public OddsCheckerMobiCouponStrategy(IBookmakerRepository bookmakerService, IWebRepository webRepository,
      IValueOptions valueOptions)
      : base(bookmakerService, webRepository, valueOptions)
    { }

    public override IEnumerable<IGenericCompetitionCoupon> GetCompetitions(OddsDownloadStage stage = OddsDownloadStage.Competition)
    {
      var tournamentsReturn = new List<IGenericCompetitionCoupon>();

      var html = this.webRepository.GetHTML(new[] { this.bookmakerService.GetCompetitionCouponUrl(this.valueOptions.Competition, this.valueOptions.OddsSource) },
        s => Console.WriteLine(s), string.Format("{0} OddsChecker Coupon", this.valueOptions.CouponDate.ToShortDateString()))
        .First();

      var oddscheckerCompetitions = WebUtils.ParseWebsite<TCompetition>(html, s => Console.WriteLine(s))
        .Cast<TCompetition>()
        .Where(c => c.CompetitionType == this.valueOptions.Competition.CompetitionName)
        .ToList();

      foreach (var t in oddscheckerCompetitions)
      {
        var competetion = new GenericCompetitionCoupon()
        {
          CompetitionName = t.CompetitionName,
          CompetitionURL = t.CompetitionURL,
        };

        if (stage != OddsDownloadStage.Competition)
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
        var matchData = new GenericMatchCoupon
        {
          MatchURL = match.MatchURL,
          TeamOrPlayerA = match.TeamOrPlayerA,
          TeamOrPlayerB = match.TeamOrPlayerB,
        };
        returnMatches.Add(matchData);
      }
      return returnMatches;
    }
  }

  public class OddsCheckerWebCouponStrategy<TCompetition> : OddsCheckerMobiCouponStrategy<TCompetition>
    where TCompetition : IOddsCheckerCompetition, new()
  {
    public OddsCheckerWebCouponStrategy(IBookmakerRepository bookmakerService, IWebRepository webRepository,
      IValueOptions valueOptions)
      : base(bookmakerService, webRepository, valueOptions)
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

          var obj = new TCompetition();
          var teamOrPlayerA = obj.ConvertTeamOrPlayerName(match.TeamOrPlayerA);
          var teamOrPlayerB = obj.ConvertTeamOrPlayerName(match.TeamOrPlayerB);


          var matchData = new GenericMatchCoupon
          {
            MatchURL = match.MatchURL,
            TeamOrPlayerA = teamOrPlayerA,
            TeamOrPlayerB = teamOrPlayerB,
            MatchDate = currentDate.AddHours(double.Parse(matchTime[0])).AddHours(double.Parse(matchTime[1]) / 60.0),
          };

          matchData.HeadlineOdds = match.BestOdds;

          returnMatches.Add(matchData);

        }
      }
      return returnMatches;
    }
  }
}
