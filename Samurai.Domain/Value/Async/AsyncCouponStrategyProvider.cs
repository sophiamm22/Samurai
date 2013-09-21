using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Model;
using Samurai.Domain.Repository;
using Samurai.Domain.HtmlElements;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.Domain.Value.Async
{
  public interface IAsyncCouponStrategyProvider
  {
    IAsyncCouponStrategy CreateCouponStrategy(IValueOptions valueOptions);
  }

  public class AsyncCouponStrategyProvider : IAsyncCouponStrategyProvider
  {
    protected readonly IBookmakerRepository bookmakerRepository;
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly IWebRepositoryProviderAsync webRepositoryProvider;

    public AsyncCouponStrategyProvider(IBookmakerRepository bookmakerService,
      IFixtureRepository fixtureRepository, IWebRepositoryProviderAsync webRepositoryProvider)
    {
      this.bookmakerRepository = bookmakerService;
      this.fixtureRepository = fixtureRepository;
      this.webRepositoryProvider = webRepositoryProvider;
    }

    public IAsyncCouponStrategy CreateCouponStrategy(IValueOptions valueOptions)
    {
      if (valueOptions.OddsSource.Source == "Best Betting")
      {
        if (valueOptions.Sport.SportName == "Football")
          return new BestBettingAsyncCouponStrategy<BestBettingCompetitionFootball>(this.bookmakerRepository,
            this.fixtureRepository, this.webRepositoryProvider, valueOptions);
        else if (valueOptions.Sport.SportName == "Tennis")
          return new BestBettingAsyncCouponStrategy<BestBettingCompetitionTennis>(this.bookmakerRepository,
            this.fixtureRepository, this.webRepositoryProvider, valueOptions);
        else
          throw new ArgumentException("Sport not recognised");
      }
      else if (valueOptions.OddsSource.Source == "Odds Checker Mobi")
      {
        if (valueOptions.Sport.SportName == "Football")
          return new OddsCheckerMobiAsyncCouponStrategy<OddsCheckerMobiCompetitionFootball>(this.bookmakerRepository,
            this.fixtureRepository, this.webRepositoryProvider, valueOptions);
        else if (valueOptions.Sport.SportName == "Tennis")
          return new OddsCheckerMobiAsyncCouponStrategy<OddsCheckerMobiCompetitionTennis>(this.bookmakerRepository,
            this.fixtureRepository, this.webRepositoryProvider, valueOptions);
        else
          throw new ArgumentException("Sport not recognised");
      }
      else if (valueOptions.OddsSource.Source == "Odds Checker Web")
      {
        if (valueOptions.Sport.SportName == "Football")
          return new OddsCheckerWebAsyncCouponStrategy<OddsCheckerWebCompetitionFootball>(this.bookmakerRepository,
            this.fixtureRepository, this.webRepositoryProvider, valueOptions);
        else if (valueOptions.Sport.SportName == "Tennis")
          return new OddsCheckerWebAsyncCouponStrategy<OddsCheckerWebCompetitionTennis>(this.bookmakerRepository,
            this.fixtureRepository, this.webRepositoryProvider, valueOptions);
        else
          throw new ArgumentException("Sport not recognised");
      }
      else
      {
        throw new ArgumentException("Odds Source not recognised");
      }
    }

  }
}
