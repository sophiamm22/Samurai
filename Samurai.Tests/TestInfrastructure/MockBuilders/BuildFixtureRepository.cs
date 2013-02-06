using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

using Samurai.SqlDataAccess.Contracts;
using E = Samurai.Domain.Entities;

namespace Samurai.Tests.TestInfrastructure.MockBuilders
{
  public static class BuildFixtureRepository
  {
    public static Mock<IFixtureRepository> Create()
    {
      return new Mock<IFixtureRepository>();
    }

    public static Mock<IFixtureRepository> HasTheSkySportsURL(this Mock<IFixtureRepository> repo, DateTime fixtureDate)
    {
      repo.Setup(x => x.GetSkySportsFootballFixturesOrResults(It.IsAny<DateTime>()))
          .Returns((DateTime d) => new Uri(string.Format("http://www1.skysports.com/football/fixtures-results/{0}", fixtureDate.ToString("dd-MMMM-yyyy"))));

      return repo;
    }

    public static Mock<IFixtureRepository> GetAliasReturnsItself(this Mock<IFixtureRepository> repo)
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

    public static Mock<IFixtureRepository> CanAddMatches(this Mock<IFixtureRepository> repo, IList<E.Match> matches)
    {
      repo.Setup(x => x.AddMatch(It.IsAny<E.Match>()))
          .Callback((E.Match m) => matches.Add(m));

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


  }
}
