using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Model;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Entities;

namespace Samurai.Domain.Value.Async
{
  public interface IAsyncPredictionStrategyProvider
  {
    IAsyncPredictionStrategy CreatePredictionStrategy(Sport sport);
  }

  public class AsyncPredictionStrategyProvider : IAsyncPredictionStrategyProvider
  {
    protected readonly IPredictionRepository predictionRepository;
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly IWebRepositoryProviderAsync webRepositoryProvider;

    public AsyncPredictionStrategyProvider(IPredictionRepository predictionRepository, IFixtureRepository fixtureRepository,
      IWebRepositoryProviderAsync webRepositoryProvider)
    {
      if (predictionRepository == null) throw new ArgumentNullException("predictionRepository");
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (webRepositoryProvider == null) throw new ArgumentNullException("webRepository");

      this.predictionRepository = predictionRepository;
      this.fixtureRepository = fixtureRepository;
      this.webRepositoryProvider = webRepositoryProvider;
    }

    public IAsyncPredictionStrategy CreatePredictionStrategy(Sport sport)
    {
      if (sport.SportName == "Football")
        return new FootballAsyncPredictionStrategy(this.predictionRepository, this.fixtureRepository, this.webRepositoryProvider);
      else if (sport.SportName == "Tennis")
        return new TennisAsyncPredictionStrategy(this.predictionRepository, this.fixtureRepository, this.webRepositoryProvider);
      else
        throw new ArgumentException("Sport not recognised");
    }
  }
}
