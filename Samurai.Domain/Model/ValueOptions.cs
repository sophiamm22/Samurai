using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Entities;

namespace Samurai.Domain.Model
{
  public interface IValueOptions
  {
    DateTime CouponDate { get; set; }
    ExternalSource OddsSource { get; set; }
    Sport Sport { get; set; }
    Tournament Tournament { get; set; }
  }

  public class ValueOptions : IValueOptions
  {
    public DateTime CouponDate { get; set; }
    public ExternalSource OddsSource { get; set; }
    public Sport Sport { get; set; }
    public Tournament Tournament { get; set; }
    public bool DontUpdateTennisStats { get; set; } //will take the default false in 99% of cases
  }
}
