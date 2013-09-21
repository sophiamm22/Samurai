using System;
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
}
