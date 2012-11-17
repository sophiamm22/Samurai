using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Domain.Model;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Entities;

namespace Samurai.Domain.Value
{
  public interface IPredictionStrategyProvider
  {
    IPredictionStrategy CreatePredictionStrategy(Sport sport);
  }

  public class PredictionStrategyProvider : IPredictionStrategyProvider
  {
    protected readonly IPredictionRepository predictionRepository;
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly IWebRepository webRepository;

    public PredictionStrategyProvider(IPredictionRepository predictionRepository, IFixtureRepository fixtureRepository,
      IWebRepository webRepository)
    {
      if (predictionRepository == null) throw new ArgumentNullException("predictionRepository");
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (webRepository == null) throw new ArgumentNullException("webRepository");

      this.predictionRepository = predictionRepository;
      this.fixtureRepository = fixtureRepository;
      this.webRepository = webRepository;
    }

    public IPredictionStrategy CreatePredictionStrategy(Sport sport)
    {
      if (sport.SportName == "Football")
        return new FootballFinkTankPredictionStrategy(this.predictionRepository, this.fixtureRepository, this.webRepository);
      else if (sport.SportName == "Tennis")
        return new TennisPredictionStrategy(this.predictionRepository, this.fixtureRepository, this.webRepository);
      else
        throw new ArgumentException("Sport not recognised");
    }
  }
}
