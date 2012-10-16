using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Web.ViewModels
{
  public class FootballFixtureSummaryViewModel
  {
    public string Season { get; set; }
    public string League { get; set; }
    public DateTime MatchDate { get; set; }
    public string Venue { get; set; }
    public string TeamsPlayerA { get; set; }
    public string TeamsPlayerB { get; set; }
    public int ScoreA { get; set; }
    public int ScoreB { get; set; }
    public int? IKTSGameWeek { get; set; }
  }
}
