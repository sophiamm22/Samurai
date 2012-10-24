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
    protected IWebRepository webRepository;
    protected M.Mock<IPredictionRepository> predictionRepository;
    protected M.Mock<IFixtureRepository> fixtureRepository;
    protected M.Mock<IValueOptions> valueOptions;
    protected DateTime couponDate;
    protected IEnumerable<IGenericPrediction> predictions;

    protected override void Establish_context()
    {
      base.Establish_context();
      this.fixtureRepository = new M.Mock<IFixtureRepository>();
      this.predictionRepository = new M.Mock<IPredictionRepository>();

      this.fixtureRepository.HasBasicMethods(this.db);
      this.predictionRepository.HasBasicMethods(this.db);
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
      this.valueOptions.Setup(t => t.Competition).Returns(this.db.Competition["ATP"]);

      this.webRepository = new WebRepositoryTestData("Tennis/" + this.couponDate.ToShortDateString().Replace("/", "-"));
      this.predictionStrategy = new TennisPredictionStrategy(this.predictionRepository.Object, this.fixtureRepository.Object, this.webRepository);

    }

    protected override void Because_of()
    {
      this.predictions = this.predictionStrategy.GetPredictions(this.valueOptions.Object).ToList();
    }

    [Test]
    public void then_a_complete_list_of_tennis_predictions_is_returned()
    {
      //need to add more tests as I get more data from tennis betting 365
      this.predictions.Count().ShouldEqual(13);
    }
  }
}
