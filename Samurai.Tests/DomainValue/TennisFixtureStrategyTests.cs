using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;

using Samurai.Domain.Value;
using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Domain.APIModel;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Repository;
using Samurai.Tests.TestInfrastructure.MockBuilders;

namespace Samurai.Tests.DomainValue
{
  public class TennisFixtureStrategyTests
  {
    public class UpdateTournamentEvents
    {
      protected List<Tournament> persistedTournaments; 
      protected List<TournamentEvent> persistedTournamentEvents;
      protected Mock<IWebRepository> webRepository;
      protected Mock<IWebRepositoryProvider> webRepositoryProvider;
      protected Mock<IFixtureRepository> fixtureRepository;

      [Test]
      public void CreatesNewListOfTournamentEvents()
      {
        //Arrange
        persistedTournaments = new List<Tournament>();
        persistedTournamentEvents = new List<TournamentEvent>();

        this.webRepository = BuildWebRepository.Create()
          .HasSingleAPITennisTourCalendar();

        this.webRepositoryProvider = BuildWebRepositoryProvider.Create()
          .ReturnsSpecificWebRepository(webRepository.Object);

        this.fixtureRepository = BuildFixtureRepository.Create()
          .HasNoPersistedTournamentEvent()
          .HasAPersistedCompetition()
          .CanAddTournamentEvent(persistedTournamentEvents)
          .CanAddTournament(persistedTournaments);

        //Act
        var fixtureStategy = new TestableTennisFixtureStrategy(this.fixtureRepository, 
          this.webRepositoryProvider);

        var tournamentEvents = fixtureStategy.UpdateTournamentEvents();
        var tournamentEvent = tournamentEvents.FirstOrDefault();
        var persistedTournament = persistedTournaments.FirstOrDefault();
        var persistedTournamentEvent = persistedTournamentEvents.FirstOrDefault();
                
        //Assert
        //Returned tournament event
        Assert.AreEqual(1, tournamentEvents.Count());
        Assert.AreEqual("Tóurnament Name (2013)", tournamentEvent.EventName);
        Assert.AreEqual(new DateTime(2012, 12, 31), tournamentEvent.StartDate);
        Assert.AreEqual(new DateTime(2013, 01, 06), tournamentEvent.EndDate);
        Assert.IsTrue(tournamentEvent.TournamentInProgress);
        Assert.IsFalse(tournamentEvent.TournamentCompleted);

        //Persisted tournament event
        Assert.AreEqual(1, persistedTournamentEvents.Count());
        Assert.AreSame(tournamentEvent, persistedTournamentEvent);

        //Persisted tournament
        Assert.AreEqual(1, persistedTournaments.Count());
        Assert.AreEqual("Tóurnament Name", persistedTournament.TournamentName);
        Assert.AreEqual("tournament-name", persistedTournament.Slug);
 
      }

      [Test]
      public void UpdatesOldListOfTournamentEvents()
      {
        //Arrange
        persistedTournaments = new List<Tournament>();
        persistedTournamentEvents = new List<TournamentEvent>();

        var persistedTournament = new Tournament
        {
          Id = 1,
          TournamentName = "Tóurnament Name",
          Slug = "tournament-name",
          CompetitionID = 1
        };

        var persistedTournamentEvent = new TournamentEvent
        {
          Tournament = persistedTournament,
          EventName = "Tóurnament Name (2013)",
          StartDate = new DateTime(2012, 12, 30),
          EndDate = new DateTime(2013, 01, 07),
          TournamentInProgress = false,
          TournamentCompleted = false
        };

        this.webRepository = BuildWebRepository.Create()
          .HasSingleAPITennisTourCalendar();

        this.webRepositoryProvider = BuildWebRepositoryProvider.Create()
          .ReturnsSpecificWebRepository(webRepository.Object);

        this.fixtureRepository = BuildFixtureRepository.Create()
          .HasAPersistedTournamentEvent(persistedTournamentEvent)
          .HasAPersistedCompetition()
          .HasAPersistedTournament(persistedTournament)
          .CanAddTournament(persistedTournaments);

        //Act
        var fixtureStrategy = new TestableTennisFixtureStrategy(this.fixtureRepository,
          this.webRepositoryProvider);

        var tournamentEvents = fixtureStrategy.UpdateTournamentEvents();
        var tournamentEvent = tournamentEvents.FirstOrDefault();

        //Assert
        //Returned tournament event
        Assert.AreEqual(1, tournamentEvents.Count());
        Assert.AreEqual("Tóurnament Name (2013)", tournamentEvent.EventName);
        Assert.AreEqual(new DateTime(2012, 12, 31), tournamentEvent.StartDate);
        Assert.AreEqual(new DateTime(2013, 01, 06), tournamentEvent.EndDate);
        Assert.IsTrue(tournamentEvent.TournamentInProgress);
        Assert.IsFalse(tournamentEvent.TournamentCompleted);

        //Tournament
        Assert.AreEqual(0, persistedTournaments.Count());
      }
    }
  }

  public class TestableTennisFixtureStrategy : NewTennisFixtureStrategy
  {
    public Mock<IFixtureRepository> MockedFixtureRepository { get; private set; }
    public Mock<IWebRepositoryProvider> MockedWebRepositoryProvider { get; private set; }

    public TestableTennisFixtureStrategy(Mock<IFixtureRepository> mockFixtureRepository,
      Mock<IWebRepositoryProvider> mockWebRepositoryProvider)
      : base(mockFixtureRepository.Object, mockWebRepositoryProvider.Object)
    {
      MockedFixtureRepository = mockFixtureRepository;
      MockedWebRepositoryProvider = mockWebRepositoryProvider;
    }
  }


}
