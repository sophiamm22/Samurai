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
