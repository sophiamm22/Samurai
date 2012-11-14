using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Value
{
  public interface IFixturesAndOdds : IFixtureStrategy, ICouponStrategy, IOddsStrategy
  {
  }
}
