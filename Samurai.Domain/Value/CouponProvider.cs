using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Domain.Model;
using Samurai.Domain.Repository;
using Samurai.Domain.HtmlElements;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.Domain.Value
{
  public interface ICouponProvider
  {
    AbstractCouponStrategy CreateCouponStrategy(IValueOptions valueOptions);
  }

  public class CouponProvider : ICouponProvider
  {
    protected readonly IBookmakerRepository bookmakerRepository;
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly IWebRepository webRepository;

    public CouponProvider(IBookmakerRepository bookmakerService, 
      IFixtureRepository fixtureRepository, IWebRepository webRepository)
    {
      this.bookmakerRepository = bookmakerService;
      this.fixtureRepository = fixtureRepository;
      this.webRepository = webRepository;
    }

    public AbstractCouponStrategy CreateCouponStrategy(IValueOptions valueOptions)
    {
      if (valueOptions.OddsSource.Source == "BestBetting")
      {
        if (valueOptions.Sport.SportName == "Football")
          return new BestBettingCouponStrategy<BestBettingCompetitionFootball>(this.bookmakerRepository, 
            this.fixtureRepository, this.webRepository, valueOptions);
        else if (valueOptions.Sport.SportName == "Tennis")
          return new BestBettingCouponStrategy<BestBettingCompetitionTennis>(this.bookmakerRepository,
            this.fixtureRepository, this.webRepository, valueOptions);
        else
          throw new ArgumentException("Sport not recognised");
      }
      else if (valueOptions.OddsSource.Source == "OddsChecker Mobi")
      {
        if (valueOptions.Sport.SportName == "Football")
          return new OddsCheckerMobiCouponStrategy<OddsCheckerMobiCompetitionFootball>(this.bookmakerRepository,
            this.fixtureRepository, this.webRepository, valueOptions);
        else if (valueOptions.Sport.SportName == "Tennis")
          return new OddsCheckerMobiCouponStrategy<OddsCheckerMobiCompetitionTennis>(this.bookmakerRepository,
            this.fixtureRepository, this.webRepository, valueOptions);
        else
          throw new ArgumentException("Sport not recognised");
      }
      else if (valueOptions.OddsSource.Source == "OddsChecker Web")
      {
        if (valueOptions.Sport.SportName == "Football")
          return new OddsCheckerWebCouponStrategy<OddsCheckerWebCompetitionFootball>(this.bookmakerRepository,
            this.fixtureRepository, this.webRepository, valueOptions);
        else if (valueOptions.Sport.SportName == "Tennis")
          return new OddsCheckerWebCouponStrategy<OddsCheckerWebCompetitionTennis>(this.bookmakerRepository,
            this.fixtureRepository, this.webRepository, valueOptions);
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
