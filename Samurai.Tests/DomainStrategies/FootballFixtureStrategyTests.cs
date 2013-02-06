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

namespace Samurai.Tests.DomainStrategies
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
      public void CreatesACollectionOfMatches()
      {
        //Arrange
        this.matches = new List<E.Match>();
        this.matchDate = new DateTime(2012, 02, 02);
        this.webRepositoryProvider = new ManifestWebRepositoryProvider();

        this.mockFixtureRepository = BuildFixtureRepository.Create()
          .HasTheSkySportsURL(this.matchDate)
          .GetAliasReturnsItself()
          .CanAddMatches(matches);

        this.mockStoredProcRepository = new Mock<IStoredProceduresRepository>();

        var footballFixtureStrategy = new TestableFootballFixtureStrategy(this.mockFixtureRepository,
          this.mockStoredProcRepository, this.webRepositoryProvider);

        //Act
        footballFixtureStrategy.UpdateFixtures(this.matchDate);

        //Assert

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
