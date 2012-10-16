using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Domain.Model;
using Samurai.Domain.Repository;

namespace Samurai.Domain.Value
{
  public interface IOddsProvider
  {
    AbstractOddsStrategy CreateOddsStrategy(IValueOptions valueOptions);
  }

  public class OddsProvider : IOddsProvider
  {
    protected readonly IWebRepository webRepository;

    public OddsProvider(IWebRepository webRepository)
    {
      this.webRepository = webRepository;
    }

    public AbstractOddsStrategy CreateOddsStrategy(IValueOptions valueOptions)
    {
      if (valueOptions.OddsSource.Source == "BestBetting")
        return new BestBettingOddsStrategy(this.webRepository);
      else if (valueOptions.OddsSource.Source == "OddsChecker Mobi")
        return new OddsCheckerMobiOddsStrategy(this.webRepository);
      else if (valueOptions.OddsSource.Source == "OddsChecker Web")
        return new OddsCheckerWebOddsStrategy(this.webRepository);
      else
        throw new ArgumentException("Odds Source not recognised");
    }
  }
}
