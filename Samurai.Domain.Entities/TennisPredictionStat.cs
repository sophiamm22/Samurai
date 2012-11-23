using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class TennisPredictionStat : BaseEntity
  {
    public TennisPredictionStat()
    {
    }

    public int MatchID { get; set; }

    public int PlayerAGames { get; set; }
    public int PlayerBGames { get; set; }
    public decimal? EPoints { get; set; }
    public decimal? EGames { get; set; }
    public decimal? ESets { get; set; }
    
    public virtual Match Match { get; set; }
  }
}
