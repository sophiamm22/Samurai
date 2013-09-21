using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model = Samurai.Domain.Model;
using Samurai.Domain.Entities;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.APIModel;

namespace Samurai.Domain.Value.Async
{
  public interface IAsyncPredictionStrategy
  {
    Task<IEnumerable<Model.GenericPrediction>> FetchPredictionsAsync(Model.IValueOptions valueOptions);
    Task<IEnumerable<Model.GenericPrediction>> FetchPredictionsCouponAsync(Model.IValueOptions valueOptions);
    Task<Model.GenericPrediction> FetchSinglePredictionAsync(TeamPlayer teamPlayerA, TeamPlayer teamPlayerB, Tournament tournament, Model.IValueOptions valueOptions);
  }

  public abstract class AbstractAsyncPredictionStrategy
  {
    protected readonly IPredictionRepository predictionRepository;
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly IWebRepositoryProviderAsync webRepositoryProvider;

    public AbstractAsyncPredictionStrategy(IPredictionRepository predictionRepository, IFixtureRepository fixtureRepository,
      IWebRepositoryProviderAsync webRepositoryProvider)
    {
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (predictionRepository == null) throw new ArgumentNullException("preictionRepository");
      if (webRepositoryProvider == null) throw new ArgumentNullException("webRepository");

      this.predictionRepository = predictionRepository;
      this.fixtureRepository = fixtureRepository;
      this.webRepositoryProvider = webRepositoryProvider;
    }
  }

}
