using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using M = Moq;
using NBehave.Spec.NUnit;
using NUnit.Framework;

using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Entities;
using Samurai.SqlDataAccess;

namespace Samurai.Tests
{
  public static partial class TestHelper
  {
    public static M.Mock<IFixtureRepository> HasBasicMethods(this M.Mock<IFixtureRepository> repo, SeedDataDictionaries db)
    {
      repo.Setup(r => r.GetSkySportsFootballFixturesOrResults(M.It.IsAny<DateTime>())).Returns((DateTime date) => 
        new Uri(string.Format("http://www1.skysports.com/football/fixtures-results/{0}", date.ToString("dd-MMMM-yyyy").ToLower())));

      repo.Setup(t => t.GetExternalSource(M.It.IsAny<string>())).Returns((string s) => db.ExternalSource[s]);

      repo.Setup(t =>
        t.GetAlias(M.It.IsAny<string>(), M.It.IsAny<ExternalSource>(), M.It.IsAny<ExternalSource>()))
         .Returns<string, ExternalSource, ExternalSource>(
         (string teamNameSource, ExternalSource exSource, ExternalSource exDestination) =>
         {
           var teamNameDestination = string.Empty;
           var teamAlias = db.TeamPlayerExternalSourceAlias
                             .Where(a => a.Alias == teamNameSource &&
                                         a.ExternalSource.Source == exSource.Source);
           if (teamAlias.Count() == 0)
             teamNameDestination = teamNameSource;
           else
             teamNameDestination = teamAlias.First().TeamsPlayer.TeamName;

           return teamNameDestination;
         });
      repo.Setup(t => t.GetTeamOrPlayer(M.It.IsAny<string>())).Returns((string s) => db.TeamsPlayer[s]);

      repo.Setup(t => t.GetCompetition(M.It.IsInRange<int>(0, 4, M.Range.Inclusive))).Returns((int id) =>
      {
        switch (id)
        {
          case 0: return db.Competition["Premier League"];
          case 1: return db.Competition["Championship"];
          case 2: return db.Competition["League One"];
          case 3: return db.Competition["League Two"];
          case 4:
          default:
            return db.Competition["ATP"];
        }
      });

      repo.Setup(t => t.GetScoreOutcome(M.It.IsAny<int>(), M.It.IsAny<int>())).Returns((int homeScore, int awayScore) =>
      {
        return db.ScoreOutcome[homeScore.ToString() + "-" + awayScore.ToString()];
      });

      return repo;
    }

    public static M.Mock<IFixtureRepository> HasNoPersistedMatches(this M.Mock<IFixtureRepository> repo)
    {
      repo.Setup(t => t.GetMatchFromTeamSelections(M.It.IsAny<TeamsPlayer>(), M.It.IsAny<TeamsPlayer>(), M.It.IsAny<DateTime>()))
          .Returns<Match>(null);
      return repo;
    }

    public static M.Mock<IFixtureRepository> HasPersistedMatches(this M.Mock<IFixtureRepository> repo)
    {
      repo.Setup(t => t.GetMatchFromTeamSelections(M.It.IsAny<TeamsPlayer>(), M.It.IsAny<TeamsPlayer>(), M.It.IsAny<DateTime>()))
          .Returns((TeamsPlayer homeTeam, TeamsPlayer awayTeam, DateTime matchDate) =>
            {
              var match = new Match
              {
                TeamsPlayerA = homeTeam,
                TeamsPlayerB = awayTeam,
                MatchDate = matchDate
              };

              return match;
            });

      return repo;
    }

  }
}
