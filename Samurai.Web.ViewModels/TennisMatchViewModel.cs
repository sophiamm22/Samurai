using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Web.ViewModels
{
  public class TennisMatchViewModel
  {
    public string TeamPlayerA { get; set; }
    public string TeamPlayerB { get; set; }
    public DateTime MatchDate { get; set; }
    public string ScoreLine { get; set; }
  }
}
