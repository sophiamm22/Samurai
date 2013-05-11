using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class ScoreOutcome : BaseEntity
  {
    public ScoreOutcome()
    {
      this.ObservedOutcomes = new List<ObservedOutcome>();
      this.ScoreOutcomeProbabilitiesInMatches = new List<ScoreOutcomeProbabilitiesInMatch>();
    }

    public int MatchOutcomeID { get; set; }
    public int TeamAScore { get; set; }
    public int TeamBScore { get; set; }
    public virtual MatchOutcome MatchOutcome { get; set; }
    public virtual ICollection<ObservedOutcome> ObservedOutcomes { get; set; }
    public virtual ICollection<ScoreOutcomeProbabilitiesInMatch> ScoreOutcomeProbabilitiesInMatches { get; set; }

    public override string ToString()
    {
      return string.Format("{0}-{1}", TeamAScore, TeamBScore);
    }
  }
}
