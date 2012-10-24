using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Web.ViewModels
{
  public class FootballFixtureViewModel
  {
    public string LeagueAndSeason { get; set; }
    public DateTime MatchDate { get; set; }
    public string TeamsPlayerA { get; set; }
    public string TeamsPlayerB { get; set; }
    public string ScoreLine { get; set; }
    public int? IKTSGameWeek { get; set; }
    
    //betting summary
    public string OutcomeBetOn { get; set; }
    public double BetValue { get; set; }
    public double OddsTaken { get; set; }
    public string Bookmaker { get; set; }
    //etc..
  }
}
