using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class ObservedOutcome : BaseEntity
  {
    public int MatchID { get; set; }
    public int ScoreOutcomeID { get; set; }
    public virtual Match Match { get; set; }
    public virtual ScoreOutcome ScoreOutcome { get; set; }
  }
}
