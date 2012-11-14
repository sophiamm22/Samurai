using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Web.ViewModels
{
  public class OddsViewModel
  {
    public double OddsBeforeCommission { get; set; }
    public double CommissionPct { get; set; }
    public double DecimalOdds { get; set; }
    public string BookmakerName { get; set; }
    public string Source { get; set; }
    public Uri ClickThroughURL { get; set; }
    public DateTime TimeStamp { get; set; }
    public int Priority { get; set; }

  }
}
