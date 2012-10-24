using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class MatchOutcomeOdd : BaseEntity
  {
    public MatchOutcomeOdd()
    {
      this.BettingPAndLs = new List<BettingPAndL>();
    }

    public int ExternalSourceID { get; set; }
    public int MatchOutcomeProbabilitiesInMatchID { get; set; }
    public int BookmakerID { get; set; }
    public DateTime TimeStamp { get; set; }
    public decimal Odd { get; set; }
    public string ClickThroughURL { get; set; }

    public virtual ICollection<BettingPAndL> BettingPAndLs { get; set; }
    public virtual Bookmaker Bookmaker { get; set; }
    public virtual ExternalSource ExternalSource { get; set; }
    public virtual MatchOutcomeProbabilitiesInMatch MatchOutcomeProbabilitiesInMatch { get; set; }
  }
}
