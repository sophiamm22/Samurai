using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class MatchOutcome : BaseEntity
  {
    public MatchOutcome()
    {
      this.MatchOutcomeProbabilitiesInMatches = new List<MatchOutcomeProbabilitiesInMatch>();
      this.ScoreOutcomes = new List<ScoreOutcome>();
    }

    public string MatchOutcomeString { get; set; }

    public virtual ICollection<MatchOutcomeProbabilitiesInMatch> MatchOutcomeProbabilitiesInMatches { get; set; }
    public virtual ICollection<ScoreOutcome> ScoreOutcomes { get; set; }
  }
}
