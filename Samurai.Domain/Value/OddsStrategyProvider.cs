using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Domain.Model;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.Domain.Value
{
  public interface IOddsStrategyProvider
  {
    IOddsStrategy CreateOddsStrategy(IValueOptions valueOptions);
  }

  public class OddsStrategyProvider : IOddsStrategyProvider
  {
    protected readonly IWebRepository webRepository;
    protected readonly IBookmakerRepository bookmakerRepository;
    protected readonly IFixtureRepository fixtureRepository;

    public OddsStrategyProvider(IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepository webRepository)
    {
      if (bookmakerRepository == null) throw new ArgumentNullException("bookmakerRepository");
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (webRepository == null) throw new ArgumentNullException("webRepository");

      this.bookmakerRepository = bookmakerRepository;
      this.fixtureRepository = fixtureRepository;
      this.webRepository = webRepository;
    }

    public IOddsStrategy CreateOddsStrategy(IValueOptions valueOptions)
    {
      if (valueOptions.OddsSource.Source == "Best Betting")
        return new BestBettingOddsStrategy(valueOptions.Sport, this.bookmakerRepository, this.fixtureRepository, this.webRepository);
      else if (valueOptions.OddsSource.Source == "Odds Checker Mobi")
        return new OddsCheckerMobiOddsStrategy(valueOptions.Sport, this.bookmakerRepository, this.fixtureRepository, this.webRepository);
      else if (valueOptions.OddsSource.Source == "Odds Checker Web")
        return new OddsCheckerWebOddsStrategy(valueOptions.Sport, this.bookmakerRepository, this.fixtureRepository, this.webRepository);
      else
        throw new ArgumentException("Odds Source not recognised");
    }
  }
}
