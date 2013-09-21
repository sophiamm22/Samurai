using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Model;
using Samurai.Domain.Entities;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Core;
using Samurai.Domain.Exceptions;
using Samurai.Domain.HtmlElements;
using Samurai.Domain.Infrastructure;

namespace Samurai.Domain.Value.Async
{
  public interface IAsyncOddsStrategy
  {
    Task<IDictionary<Outcome, IEnumerable<GenericOdd>>> GetOdds(GenericMatchCoupon matchCoupon, DateTime couponDate, DateTime timeStamp);
  }

  public abstract class AbstractAsyncOddsStrategy : IAsyncOddsStrategy
  {
    protected readonly Sport sport;
    protected readonly IWebRepositoryProviderAsync webRepositoryProvider;
    protected readonly IBookmakerRepository bookmakerRepository;
    protected readonly IFixtureRepository fixtureRepository;

    public AbstractAsyncOddsStrategy(Sport sport, IBookmakerRepository bookmakerRepository, 
      IFixtureRepository fixtureRepository, IWebRepositoryProviderAsync webRepositoryProvider)
    {
      if (sport == null) throw new ArgumentNullException("sport");
      if (bookmakerRepository == null) throw new ArgumentNullException("bookmakerRepository");
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (webRepositoryProvider == null) throw new ArgumentNullException("webRepository");

      this.sport = sport;
      this.bookmakerRepository = bookmakerRepository;
      this.fixtureRepository = fixtureRepository;
      this.webRepositoryProvider = webRepositoryProvider;
    }
    public abstract Task<IDictionary<Outcome, IEnumerable<GenericOdd>>> GetOdds(GenericMatchCoupon matchCoupon, DateTime couponDate, DateTime timeStamp);
  }

}
