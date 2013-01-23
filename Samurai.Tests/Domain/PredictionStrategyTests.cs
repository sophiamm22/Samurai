using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M = Moq;
using NBehave.Spec.NUnit;
using NUnit.Framework;

using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Value;
using Samurai.Domain.Repository;
using Samurai.Domain.Entities;
using Samurai.Domain.Model;

namespace Samurai.Tests.Domain
{
  public class when_working_with_a_prediction_strategy : Specification
  {
    protected AbstractPredictionStrategy predictionStrategy;
    protected IWebRepositoryProvider webRepositoryProvider;
    protected M.Mock<IPredictionRepository> predictionRepository;
    protected M.Mock<IFixtureRepository> fixtureRepository;
    protected M.Mock<IValueOptions> valueOptions;
    protected DateTime couponDate;
    protected IEnumerable<GenericPrediction> predictions;

    protected override void Establish_context()
    {
      base.Establish_context();
      this.fixtureRepository = new M.Mock<IFixtureRepository>();
      this.predictionRepository = new M.Mock<IPredictionRepository>();

      this.fixtureRepository.HasBasicMethods(this.db);
      this.predictionRepository.HasBasicMethods(this.db);
    }
  }

  public class and_using_the_football_prediction_strategy : when_working_with_a_prediction_strategy
  {
    protected IEnumerable<GenericPrediction> premPredictions;
    protected IEnumerable<GenericPrediction> champPredictions;
    protected IEnumerable<GenericPrediction> league1Predictions;
    protected IEnumerable<GenericPrediction> league2Predictions;

    protected override void Establish_context()
    {
      base.Establish_context();

      this.fixtureRepository.HasFinkTankIDMatches();

      this.couponDate = new DateTime(2012, 10, 20);

      valueOptions = new M.Mock<IValueOptions>();

      this.valueOptions.Setup(t => t.CouponDate).Returns(this.couponDate);
      this.valueOptions.Setup(t => t.OddsSource).Returns(this.db.ExternalSource["Fink Tank (dectech)"]);
      this.valueOptions.Setup(t => t.Sport).Returns(this.db.Sport["Football"]);

      this.webRepositoryProvider = new WebRepositoryProvider("TestData", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ValueSamurai\");
      this.predictionStrategy = new FootballFinkTankPredictionStrategy(this.predictionRepository.Object, this.fixtureRepository.Object, this.webRepositoryProvider);

    }

    protected override void Because_of()
    {
      this.valueOptions.Setup(t => t.Tournament).Returns(this.db.Tournament["Premier League"]);
      this.premPredictions = this.predictionStrategy.FetchPredictions(this.valueOptions.Object).ToList();

      this.valueOptions.Setup(t => t.Tournament).Returns(this.db.Tournament["Championship"]);
      this.champPredictions = this.predictionStrategy.FetchPredictions(this.valueOptions.Object).ToList();

      this.valueOptions.Setup(t => t.Tournament).Returns(this.db.Tournament["League One"]);
      this.league1Predictions = this.predictionStrategy.FetchPredictions(this.valueOptions.Object).ToList();

      this.valueOptions.Setup(t => t.Tournament).Returns(this.db.Tournament["League Two"]);
      this.league2Predictions = this.predictionStrategy.FetchPredictions(this.valueOptions.Object).ToList();
    }

    [Test]
    public void then_a_complete_list_of_premier_league_predictions_is_returned()
    {
      this.premPredictions.Count().ShouldEqual(8);
      this.premPredictions.Select(p => p.OutcomeProbabilities.Values.Sum()).Sum().ShouldApproximatelyEqual(8.0, 0.05);
    }

    [Test]
    public void then_a_complete_list_of_championship_predictions_is_returned()
    {
      this.champPredictions.Count().ShouldEqual(11);
      this.champPredictions.Select(p => p.OutcomeProbabilities.Values.Sum()).Sum().ShouldApproximatelyEqual(11.0, 0.05);
    }

    [Test]
    public void then_a_complete_list_of_league_1_predictions_is_returned()
    {
      this.league1Predictions.Count().ShouldEqual(12);
      this.league1Predictions.Select(p => p.OutcomeProbabilities.Values.Sum()).Sum().ShouldApproximatelyEqual(12.0, 0.05);
    }

    [Test]
    public void then_a_complete_list_of_league_2_predictions_is_returned()
    {
      this.league2Predictions.Count().ShouldEqual(11);
      this.league2Predictions.Select(p => p.OutcomeProbabilities.Values.Sum()).Sum().ShouldApproximatelyEqual(11.0, 0.05);
    }
  }

  public class and_using_the_tennis_prediction_strategy : when_working_with_a_prediction_strategy
  {
    protected override void Establish_context()
    {
      base.Establish_context();

      this.couponDate = new DateTime(2012, 08, 20);

      valueOptions = new M.Mock<IValueOptions>();

      this.valueOptions.Setup(t => t.CouponDate).Returns(this.couponDate);
      this.valueOptions.Setup(t => t.OddsSource).Returns(this.db.ExternalSource["Tennis Betting 365"]);
      this.valueOptions.Setup(t => t.Sport).Returns(this.db.Sport["Tennis"]);
      this.valueOptions.Setup(t => t.Tournament).Returns(this.db.Tournament["Western & Southern Open"]);

      this.webRepositoryProvider = new WebRepositoryProvider("TestData", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ValueSamurai\");
      this.predictionStrategy = new TennisPredictionStrategy(this.predictionRepository.Object, this.fixtureRepository.Object, this.webRepositoryProvider);

    }

    protected override void Because_of()
    {
      this.predictions = this.predictionStrategy.FetchPredictions(this.valueOptions.Object).ToList();
    }

    [Test]
    public void then_a_complete_list_of_tennis_predictions_is_returned()
    {
      //need to add more tests as I get more data from tennis betting 365
      this.predictions.Count().ShouldEqual(13);
    }
  }
}
