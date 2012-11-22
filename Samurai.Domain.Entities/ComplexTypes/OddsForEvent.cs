using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Entities.ComplexTypes
{
  public class OddsForEvent
  {
    public string Outcome { get; set; }
    public decimal Probability { get; set; }
    public string OddsSource { get; set; }
    public string Bookmaker { get; set; }
    public decimal Odds { get; set; }
    public decimal Edge { get; set; }
    public DateTime TimeStamp { get; set; }
    public Uri ClickThroughURL { get; set; }
  }
}
