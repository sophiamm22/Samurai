using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samurai.Domain.Entities.ComplexTypes
{
  public class DaysFootballPredictions
  {
    public int MatchID_pk { get; set; }
    public string TournamentName { get; set; }
    public int Year { get; set; }
    public DateTime MatchDate { get; set; }
    public string TeamA { get; set; }
    public string TeamB { get; set; }
    public decimal TeamAProbability { get; set; }
    public decimal Draw { get; set; }
    public decimal TeamBProbability { get; set; }
    public string Score { get; set; }
  }
}
