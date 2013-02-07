using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;

using Samurai.Domain.Value;
using E = Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Domain.APIModel;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Repository;
using Samurai.Tests.TestInfrastructure;
using Samurai.Tests.TestInfrastructure.MockBuilders;

namespace Samurai.Tests.DomainValue
{
  public class FootballFixtureStrategyTests
  {
    public class UpdateFixtures
    {
      private DateTime matchDate;
      private Mock<IFixtureRepository> mockFixtureRepository;
      private IWebRepositoryProvider webRepositoryProvider;
      private Mock<IStoredProceduresRepository> mockStoredProcRepository;
      private List<E.Match> matches;

      [Test]
      public void CreatesANewCollectionOfMatches()
      {
        //Arrange
        this.matches = new List<E.Match>();
        this.matchDate = new DateTime(2013, 02, 02);
        this.webRepositoryProvider = new ManifestWebRepositoryProvider();

        this.mockFixtureRepository = BuildFixtureRepository.Create()
          .HasTheSkySportsURL(this.matchDate)
          .HasGetAliasWhichReturnsItself()
          .CanAddOrUpdateMatches(matches)
          .HasFootballTournamentEvents();

        this.mockStoredProcRepository = new Mock<IStoredProceduresRepository>();

        var footballFixtureStrategy = new TestableFootballFixtureStrategy(this.mockFixtureRepository,
          this.mockStoredProcRepository, this.webRepositoryProvider);

        //Act
        footballFixtureStrategy.UpdateFixtures(this.matchDate);

        //Assert
        //We have the right number of matches
        Assert.AreEqual(8, this.matches.Count(x => x.TournamentEvent.Id == 1)); //Prem
        Assert.AreEqual(11, this.matches.Count(x => x.TournamentEvent.Id == 2)); //Champ
        Assert.AreEqual(11, this.matches.Count(x => x.TournamentEvent.Id == 3)); //League 1
        Assert.AreEqual(10, this.matches.Count(x => x.TournamentEvent.Id == 4)); //League 2
        //We have collected the correct dates
        Assert.AreEqual(1, this.matches.Count(x => x.MatchDate == this.matchDate.AddHours(12.75)));
        Assert.AreEqual(1, this.matches.Count(x => x.MatchDate == this.matchDate.AddHours(17.5)));
        Assert.AreEqual(1, this.matches.Count(x => x.MatchDate == this.matchDate.AddHours(17).AddMinutes(20)));
        Assert.AreEqual(37, this.matches.Count(x => x.MatchDate == this.matchDate.AddHours(15)));
        //Spot check a few matches
        Assert.AreEqual(1, this.matches.Count(x => x.TeamsPlayerA.Name == "QPR" && x.TeamsPlayerB.Name == "Norwich"));
        Assert.AreEqual(1, this.matches.Count(x => x.TeamsPlayerA.Name == "Yeovil" && x.TeamsPlayerB.Name == "Brentford"));
        Assert.AreEqual(1, this.matches.Count(x => x.TeamsPlayerA.Name == "Cheltenham" && x.TeamsPlayerB.Name == "Torquay"));
      }

      [Test]
      public void UpdatesAnExistingMatch()
      {
        //Arrange
        this.matches = new List<E.Match>();
        this.matchDate = new DateTime(2013, 02, 02);

        var matchToCheck = new E.Match()
        {
          TeamsPlayerA = new E.TeamPlayer { Name = "QPR" },
          TeamsPlayerB = new E.TeamPlayer { Name = "Norwich" },
          MatchDate = this.matchDate.AddHours(15) //wrong time
        };

        matches.Add(matchToCheck);

        this.webRepositoryProvider = new ManifestWebRepositoryProvider();

        this.mockFixtureRepository = BuildFixtureRepository.Create()
          .HasTheSkySportsURL(this.matchDate)
          .HasGetAliasWhichReturnsItself()
          .CanAddOrUpdateMatches(matches)
          .HasFootballTournamentEvents();

        this.mockStoredProcRepository = new Mock<IStoredProceduresRepository>();

        var footballFixtureStrategy = new TestableFootballFixtureStrategy(this.mockFixtureRepository,
          this.mockStoredProcRepository, this.webRepositoryProvider);

        //Act
        footballFixtureStrategy.UpdateFixtures(this.matchDate);

        //Assert
        //Hasn't been added twice
        Assert.AreEqual(1, this.matches.Count(x => x.TeamsPlayerA.Name == "QPR" && x.TeamsPlayerB.Name == "Norwich"));
        //Time has been updated
        Assert.AreEqual(this.matchDate.AddHours(12.75), this.matches.First(x => x.TeamsPlayerA.Name == "QPR" && x.TeamsPlayerB.Name == "Norwich").MatchDate);
      }
    }

    public class UpdateResults
    {
      private DateTime matchDate;
      private Mock<IFixtureRepository> mockFixtureRepository;
      private IWebRepositoryProvider webRepositoryProvider;
      private Mock<IStoredProceduresRepository> mockStoredProcRepository;
      private List<E.Match> matches;

      [Test]
      public void CreatesANewCollectionOfMatchesWithResults()
      {
        //Arrange
        this.matches = new List<E.Match>();
        this.matchDate = new DateTime(2013, 02, 02);
        this.webRepositoryProvider = new ManifestWebRepositoryProvider();

        this.mockFixtureRepository = BuildFixtureRepository.Create()
          .HasTheSkySportsURL(this.matchDate)
          .HasGetAliasWhichReturnsItself()
          .CanAddOrUpdateMatches(matches)
          .HasFootballTournamentEvents()
          .CanReturnScoreOutcome();

        this.mockStoredProcRepository = new Mock<IStoredProceduresRepository>();

        var footballFixtureStrategy = new TestableFootballFixtureStrategy(this.mockFixtureRepository,
          this.mockStoredProcRepository, this.webRepositoryProvider);

        //Act
        footballFixtureStrategy.UpdateResults(this.matchDate);

        //Assert
        //We have the right number of matches
        Assert.AreEqual(8, this.matches.Count(x => x.TournamentEvent.Id == 1)); //Prem
        Assert.AreEqual(11, this.matches.Count(x => x.TournamentEvent.Id == 2)); //Champ
        Assert.AreEqual(11, this.matches.Count(x => x.TournamentEvent.Id == 3)); //League 1
        Assert.AreEqual(10, this.matches.Count(x => x.TournamentEvent.Id == 4)); //League 2
        //We have collected the correct dates
        Assert.AreEqual(1, this.matches.Count(x => x.MatchDate == this.matchDate.AddHours(12.75)));
        Assert.AreEqual(1, this.matches.Count(x => x.MatchDate == this.matchDate.AddHours(17.5)));
        Assert.AreEqual(1, this.matches.Count(x => x.MatchDate == this.matchDate.AddHours(17).AddMinutes(20)));
        Assert.AreEqual(37, this.matches.Count(x => x.MatchDate == this.matchDate.AddHours(15)));
        //Spot check a few matches
        var qprNowich = this.matches.Where(x => x.TeamsPlayerA.Name == "QPR" && x.TeamsPlayerB.Name == "Norwich");
        var yeovilBrentford = this.matches.Where(x => x.TeamsPlayerA.Name == "Yeovil" && x.TeamsPlayerB.Name == "Brentford");
        var cheltenhamTorquay = this.matches.Where(x => x.TeamsPlayerA.Name == "Cheltenham" && x.TeamsPlayerB.Name == "Torquay");
        
        Assert.AreEqual(1, qprNowich.Count());
        Assert.AreEqual(1, yeovilBrentford.Count());
        Assert.AreEqual(1, cheltenhamTorquay.Count());

        Assert.AreEqual("0-0", qprNowich.FirstOrDefault().ObservedOutcomes.First().ScoreOutcome.ToString());
        Assert.AreEqual("3-0", yeovilBrentford.FirstOrDefault().ObservedOutcomes.First().ScoreOutcome.ToString());
        Assert.AreEqual("2-1", cheltenhamTorquay.FirstOrDefault().ObservedOutcomes.First().ScoreOutcome.ToString());
      }

      [Test]
      public void AddScoresToAnExistingFixture()
      {
        //Arrange
        this.matches = new List<E.Match>();
        this.matchDate = new DateTime(2013, 02, 02);

        var matchToCheck = new E.Match()
        {
          TeamsPlayerA = new E.TeamPlayer { Name = "QPR" },
          TeamsPlayerB = new E.TeamPlayer { Name = "Norwich" },
          MatchDate = this.matchDate.AddHours(15), //wrong time
          TournamentEvent = new E.TournamentEvent { Id = 1 }
        };

        matches.Add(matchToCheck);

        this.webRepositoryProvider = new ManifestWebRepositoryProvider();

        this.mockFixtureRepository = BuildFixtureRepository.Create()
          .HasTheSkySportsURL(this.matchDate)
          .HasGetAliasWhichReturnsItself()
          .CanAddOrUpdateMatches(matches)
          .HasPersistedMatches(matches)
          .HasFootballTournamentEvents()
          .CanReturnScoreOutcome();

        this.mockStoredProcRepository = new Mock<IStoredProceduresRepository>();

        var footballFixtureStrategy = new TestableFootballFixtureStrategy(this.mockFixtureRepository,
          this.mockStoredProcRepository, this.webRepositoryProvider);

        //Act
        footballFixtureStrategy.UpdateResults(this.matchDate);

        //Assert
        //We have the right number of matches
        Assert.AreEqual(8, this.matches.Count(x => x.TournamentEvent.Id == 1)); //Prem
        Assert.AreEqual(11, this.matches.Count(x => x.TournamentEvent.Id == 2)); //Champ
        Assert.AreEqual(11, this.matches.Count(x => x.TournamentEvent.Id == 3)); //League 1
        Assert.AreEqual(10, this.matches.Count(x => x.TournamentEvent.Id == 4)); //League 2
        //We have collected the correct dates
        Assert.AreEqual(1, this.matches.Count(x => x.MatchDate == this.matchDate.AddHours(12.75)));
        Assert.AreEqual(1, this.matches.Count(x => x.MatchDate == this.matchDate.AddHours(17.5)));
        Assert.AreEqual(1, this.matches.Count(x => x.MatchDate == this.matchDate.AddHours(17).AddMinutes(20)));
        Assert.AreEqual(37, this.matches.Count(x => x.MatchDate == this.matchDate.AddHours(15)));
        //Spot check a few matches
        var qprNowich = this.matches.Where(x => x.TeamsPlayerA.Name == "QPR" && x.TeamsPlayerB.Name == "Norwich");
        var yeovilBrentford = this.matches.Where(x => x.TeamsPlayerA.Name == "Yeovil" && x.TeamsPlayerB.Name == "Brentford");
        var cheltenhamTorquay = this.matches.Where(x => x.TeamsPlayerA.Name == "Cheltenham" && x.TeamsPlayerB.Name == "Torquay");

        Assert.AreEqual(1, qprNowich.Count());
        Assert.AreEqual(1, yeovilBrentford.Count());
        Assert.AreEqual(1, cheltenhamTorquay.Count());

        Assert.AreEqual("0-0", qprNowich.FirstOrDefault().ObservedOutcomes.First().ScoreOutcome.ToString());
        Assert.AreEqual("3-0", yeovilBrentford.FirstOrDefault().ObservedOutcomes.First().ScoreOutcome.ToString());
        Assert.AreEqual("2-1", cheltenhamTorquay.FirstOrDefault().ObservedOutcomes.First().ScoreOutcome.ToString());
      }
    }
  }

  public class TestableFootballFixtureStrategy : NewFootballFixtureStrategy
  {
    public TestableFootballFixtureStrategy(Mock<IFixtureRepository> mockFixtureRepository,
      Mock<IStoredProceduresRepository> mockStoredProcRepository, IWebRepositoryProvider webRepositoryProvider)
      : base(mockFixtureRepository.Object, mockStoredProcRepository.Object, webRepositoryProvider)
    {

    }
  }

}
