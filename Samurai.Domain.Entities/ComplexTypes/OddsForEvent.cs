using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Entities.ComplexTypes
{
  public class OddsForEvent //bit pointless as this is essentially just a OddViewModel with additional easily calculated fields
  {
    public int MatchId { get; set; }
    public bool IsBetable { get; set; }
    public string Outcome { get; set; }
    public decimal OddBeforeCommission { get; set; }
    public double? CommissionPct { get; set; }
    public double DecimalOdd { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Bookmaker { get; set; }
    public string OddsSource { get; set; }
    public string ClickThroughURL { get; set; }
    public int Priority { get; set; }

    public string MatchCouponURL { get; set; }
    public double BookmakerID { get; set; }
    public decimal Edge { get; set; }
    public decimal Probability { get; set; }

    public override string ToString()
    {
      return string.Format("{0}-{1} {2} ({3}) @ {4}", Outcome, OddBeforeCommission, Bookmaker, OddsSource, TimeStamp.ToShortTimeString());

    }
  }
}
