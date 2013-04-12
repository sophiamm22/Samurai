using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Model;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.Domain.Value.Async
{
  public interface IAsyncOddsStrategyProvider
  {
    IAsyncOddsStrategy CreateOddsStrategy(IValueOptions valueOptions);
  }

  public class AsyncOddsStrategyProvider : IAsyncOddsStrategyProvider
  {
    protected readonly IWebRepositoryProviderAsync webRepositoryProvider;
    protected readonly IBookmakerRepository bookmakerRepository;
    protected readonly IFixtureRepository fixtureRepository;

    public AsyncOddsStrategyProvider(IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepositoryProviderAsync webRepositoryProvider)
    {
      if (bookmakerRepository == null) throw new ArgumentNullException("bookmakerRepository");
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (webRepositoryProvider == null) throw new ArgumentNullException("webRepository");

      this.bookmakerRepository = bookmakerRepository;
      this.fixtureRepository = fixtureRepository;
      this.webRepositoryProvider = webRepositoryProvider;
    }

    public IAsyncOddsStrategy CreateOddsStrategy(IValueOptions valueOptions)
    {
      if (valueOptions.OddsSource.Source == "Best Betting")
        return new BestBettingAsyncOddsStrategy(valueOptions.Sport, this.bookmakerRepository, this.fixtureRepository, this.webRepositoryProvider);
      else if (valueOptions.OddsSource.Source == "Odds Checker Mobi")
        return new OddsCheckerMobiAsyncOddsStrategy(valueOptions.Sport, this.bookmakerRepository, this.fixtureRepository, this.webRepositoryProvider);
      else if (valueOptions.OddsSource.Source == "Odds Checker Web")
        return new OddsCheckerWebAsyncOddsStrategy(valueOptions.Sport, this.bookmakerRepository, this.fixtureRepository, this.webRepositoryProvider);
      else
        throw new ArgumentException("Odds Source not recognised");
    }
  }
}
