using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class Bookmaker : BaseEntity
  {
    public Bookmaker()
    {
      this.MatchOutcomeOdds = new List<MatchOutcomeOdd>();
    }

    public string BookmakerName { get; set; }
    public string Slug { get; set; }
    public string BookmakerURL { get; set; }
    public string BookmakerNotes { get; set; }
    public bool IsExchange { get; set; }
    public decimal? CurrentCommission { get; set; }
    public decimal? BookmakerBalance { get; set; }
    public string OddsCheckerShortID { get; set; }
    public virtual ICollection<MatchOutcomeOdd> MatchOutcomeOdds { get; set; }
  }
}
