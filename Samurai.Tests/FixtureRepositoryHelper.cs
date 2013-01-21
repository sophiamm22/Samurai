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

      //repo.Setup(t =>
      //  t.GetAlias(M.It.IsAny<string>(), M.It.IsAny<ExternalSource>(), M.It.IsAny<ExternalSource>(), M.It.IsAny<Sport>()))
      //   .Returns<string, ExternalSource, ExternalSource>(
      //   (string teamNameSource, ExternalSource exSource, ExternalSource exDestination) =>
      //   {
      //     var teamNameDestination = string.Empty;
      //     var teamAlias = db.TeamPlayerExternalSourceAlias
      //                       .Where(a => a.Alias == teamNameSource &&
      //                                   a.ExternalSource.Source == exSource.Source);
      //     if (teamAlias.Count() == 0)
      //       teamNameDestination = teamNameSource;
      //     else
      //       teamNameDestination = teamAlias.First().TeamsPlayer.Name;

      //     return teamNameDestination;
      //   });
      repo.Setup(t => t.GetTeamOrPlayer(M.It.IsAny<string>())).Returns((string s) => db.TeamsPlayer[s]);

      repo.Setup(t => t.GetCompetitionById(M.It.IsInRange<int>(0, 4, M.Range.Inclusive))).Returns((int id) =>
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
      repo.Setup(t => t.GetMatchFromTeamSelections(M.It.IsAny<TeamPlayer>(), M.It.IsAny<TeamPlayer>(), M.It.IsAny<DateTime>()))
          .Returns<Match>(null);
      return repo;
    }

    public static M.Mock<IFixtureRepository> HasPersistedMatches(this M.Mock<IFixtureRepository> repo)
    {
      repo.Setup(t => t.GetMatchFromTeamSelections(M.It.IsAny<TeamPlayer>(), M.It.IsAny<TeamPlayer>(), M.It.IsAny<DateTime>()))
          .Returns((TeamPlayer homeTeam, TeamPlayer awayTeam, DateTime matchDate) =>
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

    public static M.Mock<IFixtureRepository> HasFinkTankIDMatches(this M.Mock<IFixtureRepository> repo)
    {
      repo.Setup(t => t.GetDaysMatches(M.It.IsAny<string>(), M.It.IsAny<DateTime>())).Returns((string comp, DateTime date) =>
        {
          var returnList = new List<Match>();

          int[,] theseTeams;
          int[,] premTeams = new int[,] { { 17, 6 }, { 8, 1 }, { 10, 35 }, { 12, 39 }, { 88, 66 }, { 18, 11 }, { 19, 15 }, { 31, 0 } };
          int[,] champTeams = new int[,] { { 2, 29 }, { 4, 47 }, { 21, 13 }, { 5, 44 }, { 24, 30 }, { 25, 3 }, { 53, 43 }, { 77, 28 }, { 32, 48 }, { 41, 59 }, { 22, 45 } };
          int[,] l1Teams = new int[,] { { 69, 65 }, { 51, 73 }, { 23, 57 }, { 95, 46 }, { 76, 2004 }, { 42, 2015 }, { 58, 79 }, { 33, 86 }, { 34, 37 }, { 64, 85 }, { 40, 52 }, { 96, 71 } };
          int[,] l2Teams = new int[,] { { 2000, 36 }, { 20, 49 }, { 70, 89 }, { 75, 50 }, { 2033, 2029 }, { 26, 2002 }, { 2009, 87 }, { 82, 97 }, { 60, 83 }, { 61, 67 }, { 91, 2005 } };

          switch (comp)
          {
            case "Premier League": theseTeams = premTeams; break;
            case "Championship": theseTeams = champTeams; break;
            case "League One": theseTeams = l1Teams; break;
            case "League Two":
            default: theseTeams = l2Teams; break;
          }

          for (int i = 0; i < theseTeams.GetLength(0); i++)
          {
            var homeTeam = new TeamPlayer { ExternalID = theseTeams[i, 0].ToString() };
            var awayTeam = new TeamPlayer { ExternalID = theseTeams[i, 1].ToString() };
            var match = new Match { TeamsPlayerA = homeTeam, TeamsPlayerB = awayTeam };

            returnList.Add(match);
          }
          return returnList;
        });
      return repo;
    }

  }
}
