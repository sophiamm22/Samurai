using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class ScoreOutcomeProbabilitiesInMatch : BaseEntity
  {
    //public int ScoreOutcomeProbabilitiesInMatchID_pk { get; set; }
    public int MatchID { get; set; }
    public int ScoreOutcomeID { get; set; }
    public decimal? ScoreOutcomeProbability { get; set; }
    public virtual Match Match { get; set; }
    public virtual ScoreOutcome ScoreOutcome { get; set; }
  }
}
