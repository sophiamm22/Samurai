using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Model;
using E = Samurai.Domain.Entities;

namespace Samurai.Tests.TestInfrastructure.MockBuilders
{
  public static class BuildFixtureRepository
  {
    public static Mock<IFixtureRepository> Create()
    {
      return new Mock<IFixtureRepository>();
    }

    public static Mock<IFixtureRepository> CanGetTournamentEventFromTournamentAndDate(this Mock<IFixtureRepository> repo)
    {
      repo.Setup(x => x.GetTournamentEventFromTournamentAndDate(It.IsAny<DateTime>(), It.IsAny<string>()))
          .Returns((DateTime date, string tournament) =>
            {
              return new E.TournamentEvent()
              {
                EventName = string.Format("{0} near to {1}", tournament, date.ToShortDateString())
              };
            });
      return repo;
    }

    public static Mock<IFixtureRepository> HasFullDaysMatchesByCompetition(this Mock<IFixtureRepository> repo, IList<E.Match> matches)
    {
      repo.Setup(x => x.GetDaysMatches(It.IsAny<string>(), It.IsAny<DateTime>()))
          .Returns((string tournament, DateTime couponDate) =>
            {
              return matches.Where(x => x.TournamentEvent.Tournament.TournamentName == tournament)
                            .Where(x => x.MatchDate.Date == couponDate.Date);
            });
      return repo;
    }

    public static Mock<IFixtureRepository> HasPersistedMatches(this Mock<IFixtureRepository> repo, IList<E.Match> matches)
    {
      repo.Setup(x => x.GetMatchFromTeamSelections(It.IsAny<E.TeamPlayer>(), It.IsAny<E.TeamPlayer>(), It.IsAny<DateTime>()))
          .Returns((E.TeamPlayer teamA, E.TeamPlayer teamB, DateTime startDate) =>
            {
              return matches.FirstOrDefault(x => x.TeamsPlayerA.Name == teamA.Name && x.TeamsPlayerB.Name == teamB.Name);
            });
      return repo;
    }

    public static Mock<IFixtureRepository> CanReturnScoreOutcome(this Mock<IFixtureRepository> repo)
    {
      repo.Setup(x => x.GetScoreOutcome(It.IsAny<int>(), It.IsAny<int>(), null))
          .Returns((int a, int b) =>
            {
              return new E.ScoreOutcome
              {
                TeamAScore = a,
                TeamBScore = b
              };
            });

      return repo;
    }

    public static Mock<IFixtureRepository> HasTheSkySportsURL(this Mock<IFixtureRepository> repo, DateTime fixtureDate)
    {
      repo.Setup(x => x.GetSkySportsFootballFixturesOrResults(It.IsAny<DateTime>()))
          .Returns((DateTime d) => new Uri(string.Format("http://www1.skysports.com/football/fixtures-results/{0}", fixtureDate.ToString("dd-MMMM-yyyy"))));

      return repo;
    }

    public static Mock<IFixtureRepository> HasFootballTournamentEvents(this Mock<IFixtureRepository> repo)
    {
      repo.Setup(x => x.GetFootballTournamentEvent(It.IsInRange(1, 4, Range.Inclusive), It.IsAny<DateTime>()))
          .Returns((int e, DateTime date) =>
            {
              return new E.TournamentEvent
              {
                Id = e,
                EventName = Enum.GetName(typeof(LeagueEnum), e)
              };
            });
      return repo;
    }

    public static Mock<IFixtureRepository> HasGetAliasWhichReturnsItself(this Mock<IFixtureRepository> repo)
    {
      repo.Setup(x => x.GetAlias(It.IsAny<string>(), It.IsAny<E.ExternalSource>(), It.IsAny<E.ExternalSource>(), It.IsAny<E.Sport>()))
          .Returns((string name, E.ExternalSource source, E.ExternalSource destination, E.Sport sport) =>
            {
              return new E.TeamPlayer
              {
                Name = name
              };
            });
      return repo;
    }

    public static Mock<IFixtureRepository> CanAddOrUpdateMatches(this Mock<IFixtureRepository> repo, IList<E.Match> matches)
    {
      repo.Setup(x => x.AddMatch(It.IsAny<E.Match>()))
          .Callback((E.Match m) =>
            {
              var persistedMatch = matches.FirstOrDefault(x => x.TeamsPlayerA.Name == m.TeamsPlayerA.Name &&
                                                               x.TeamsPlayerB.Name == m.TeamsPlayerB.Name);
              if (persistedMatch == null)
                matches.Add(m);
              else
                persistedMatch.MatchDate = m.MatchDate;
            });

      return repo;
    }

    public static Mock<IFixtureRepository> HasAPersistedTournamentEvent(this Mock<IFixtureRepository> repo, E.TournamentEvent tournamentEvent)
    {
      repo.Setup(x => x.GetTournamentEventFromTournamentAndYear(It.IsAny<int>(), It.IsAny<string>()))
          .Returns(() => tournamentEvent);

      return repo;
    }

    public static Mock<IFixtureRepository> HasNoPersistedTournamentEvent(this Mock<IFixtureRepository> repo)
    {
      repo.Setup(x => x.GetTournamentEventFromTournamentAndYear(It.IsAny<int>(), It.IsAny<string>()))
          .Returns(() => null);

      return repo;
    }

    public static Mock<IFixtureRepository> HasAPersistedTournament(this Mock<IFixtureRepository> repo, E.Tournament tournament)
    {
      repo.Setup(x => x.GetTournament(It.IsAny<string>()))
          .Returns(() => tournament);
      return repo;
    }

    public static Mock<IFixtureRepository> CanAddTournamentEvent(this Mock<IFixtureRepository> repo, IList<E.TournamentEvent> tournamentEvents)
    {
      repo.Setup(x => x.AddTournamentEvent(It.IsAny<E.TournamentEvent>()))
          .Callback((E.TournamentEvent t) => tournamentEvents.Add(t));

      return repo;
    }

    public static Mock<IFixtureRepository> CanAddTournament(this Mock<IFixtureRepository> repo, IList<E.Tournament> tournaments)
    {
      repo.Setup(x => x.CreateTournament(It.IsAny<E.Tournament>()))
          .Callback((E.Tournament t) => tournaments.Add(t));
      return repo;
    }

    public static Mock<IFixtureRepository> HasAPersistedCompetition(this Mock<IFixtureRepository> repo)
    {
      repo.Setup(x => x.GetCompetition(It.IsAny<string>()))
          .Returns((string c) =>
            new E.Competition
            {
              CompetitionName = c,
              Id = 1
            });
      return repo;
    }

    public static Mock<IFixtureRepository> CanGetExternalSource(this Mock<IFixtureRepository> repo)
    {
      repo.Setup(x => x.GetExternalSource(It.IsAny<string>()))
          .Returns((string sourceName) =>
          {
            return new E.ExternalSource() { Source = sourceName };
          });
      return repo;
    }

  }
}
