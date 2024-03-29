using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class MatchOutcomeProbabilitiesInMatch : BaseEntity
  {
    public MatchOutcomeProbabilitiesInMatch()
    {
      this.MatchOutcomeOdds = new List<MatchOutcomeOdd>();
    }

    public int MatchID { get; set; }
    public int MatchOutcomeID { get; set; }
    public decimal MatchOutcomeProbability { get; set; }

    public virtual Match Match { get; set; }
    public virtual ICollection<MatchOutcomeOdd> MatchOutcomeOdds { get; set; }
    public virtual MatchOutcome MatchOutcome { get; set; }
  }
}
