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
using Model = Samurai.Domain.Model;

namespace Samurai.Tests.DomainValue
{
  public class TennisPredictionStrategyTests
  {
    public class FetchPredictions
    {
      private DateTime matchDate;
      private Mock<IFixtureRepository> mockFixtureRepository;
      private IWebRepositoryProvider webRepositoryProvider;
      private Mock<IPredictionRepository> mockPredictionRepository;
      private List<E.Match> matches;

      [Test, Category("TennisPredictionStrategyTests.FetchPredictions")]
      public void CreatesACollectionOfPredictionsForTodaysMatches()
      {
        //Arrange
        matchDate = new DateTime(2013, 02, 06);
        this.webRepositoryProvider = new ManifestWebRepositoryProvider();

        this.mockFixtureRepository = BuildFixtureRepository.Create();

        this.mockPredictionRepository = BuildPredictionRepository.Create()
          .HasTodaysMatchesURL();

        var predictionStrategy = new TennisPredictionStrategy(this.mockPredictionRepository.Object,
          this.mockFixtureRepository.Object, this.webRepositoryProvider);

        var valueOptions = new Model.ValueOptions()
        {
          Sport = new E.Sport { SportName = "Tennis" },
          OddsSource = new E.ExternalSource { Source = "Not relevant" },
          CouponDate = matchDate,
          Tournament = new E.Tournament { TournamentName = "Not relevant" }
        };

        //Act
        var genericPredictions = predictionStrategy.FetchPredictions(valueOptions);

        //Assert
        Assert.AreEqual(18, genericPredictions.Count());
        genericPredictions.ToList().ForEach(x =>
          {
            Assert.AreEqual(x.OutcomeProbabilities.Sum(o => o.Value), 1.0, 0.01);
          });
        //spot check
        Assert.AreEqual(1, genericPredictions.Count(x => x.TeamOrPlayerA == "Ramos" && x.TeamOrPlayerB == "Dutra Silva"));
        Assert.AreEqual(1, genericPredictions.Count(x => x.TeamOrPlayerA == "Sousa" && x.TeamOrPlayerB == "Gimeno-Traver"));
        Assert.AreEqual(1, genericPredictions.Count(x => x.TeamOrPlayerA == "Davydenko" && x.TeamOrPlayerB == "Nieminen"));
      }
    }
  }
}
