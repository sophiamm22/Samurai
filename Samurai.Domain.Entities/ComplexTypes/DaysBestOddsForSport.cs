using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;


namespace Samurai.Domain.Entities.ComplexTypes
{
  public class DaysBestOddsForSport
  {
    public int MatchID_pk { get; set; }
    public string SportName { get; set; }
    public string MatchOutcome { get; set; }
    public decimal Odd { get; set; }
    public decimal? CurrentCommission { get; set; }
    public decimal OddAfterCommission { get; set; }
    public string BookmakerName { get; set; }
    public string Source { get; set; }
    public string ClickThroughURL { get; set; }
    public DateTime TimeStamp { get; set; }
    public int Priority { get; set; }
  }

}
