using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class BettingPAndL : BaseEntity
  {
    public int MatchOutcomeOddID { get; set; }
    public bool Cancelled { get; set; }
    public double Overround { get; set; }
    public Nullable<decimal> Stake { get; set; }
    public Nullable<decimal> CommisionPaid { get; set; }
    public Nullable<decimal> ProfitLossAfterCommision { get; set; }
    public Nullable<decimal> OddsTakenOverride { get; set; }
    public virtual MatchOutcomeOdd MatchOutcomeOdd { get; set; }
  }
}
