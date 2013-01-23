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
    protected readonly IWebRepositoryProvider webRepositoryProvider;

    public PredictionStrategyProvider(IPredictionRepository predictionRepository, IFixtureRepository fixtureRepository,
      IWebRepositoryProvider webRepositoryProvider)
    {
      if (predictionRepository == null) throw new ArgumentNullException("predictionRepository");
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (webRepositoryProvider == null) throw new ArgumentNullException("webRepository");

      this.predictionRepository = predictionRepository;
      this.fixtureRepository = fixtureRepository;
      this.webRepositoryProvider = webRepositoryProvider;
    }

    public IPredictionStrategy CreatePredictionStrategy(Sport sport)
    {
      if (sport.SportName == "Football")
        return new FootballFinkTankPredictionStrategy(this.predictionRepository, this.fixtureRepository, this.webRepositoryProvider);
      else if (sport.SportName == "Tennis")
        return new TennisPredictionStrategy(this.predictionRepository, this.fixtureRepository, this.webRepositoryProvider);
      else
        throw new ArgumentException("Sport not recognised");
    }
  }
}
