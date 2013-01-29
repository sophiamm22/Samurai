using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Web.ViewModels.Value
{
  public class OddViewModel
  {
    public bool IsBetable { get; set; }
    public string Outcome { get; set; }
    public double OddBeforeCommission { get; set; }
    public double? CommissionPct { get; set; }
    public double DecimalOdd { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Bookmaker { get; set; }
    public string OddsSource { get; set; }
    public string ClickThroughURL { get; set; }
    public int Priority { get; set; }
  }
}
