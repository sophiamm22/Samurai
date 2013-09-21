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


}
