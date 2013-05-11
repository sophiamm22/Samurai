using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class MatchOutcome : BaseEntity
  {
    public MatchOutcome()
    {
      //this.BettingPAndLs = new List<BettingPAndL>();
      this.MatchOutcomeProbabilitiesInMatches = new List<MatchOutcomeProbabilitiesInMatch>();
      this.ScoreOutcomes = new List<ScoreOutcome>();
    }

    //public int MatchOutcomeID_pk { get; set; }
    public string MatchOutcomeString { get; set; }
    //public virtual ICollection<BettingPAndL> BettingPAndLs { get; set; }
    public virtual ICollection<MatchOutcomeProbabilitiesInMatch> MatchOutcomeProbabilitiesInMatches { get; set; }
    public virtual ICollection<ScoreOutcome> ScoreOutcomes { get; set; }
  }
}
