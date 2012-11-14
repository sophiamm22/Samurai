using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Web.ViewModels
{
  public class MatchCouponViewModel
  {
    DateTime MatchDate { get; set; }
    string TeamOrPlayerA { get; set; }
    string TeamOrPlayerB { get; set; }
    Uri MatchURL { get; set; }
    IDictionary<string, double> HeadlineOdds { get; set; }
    IDictionary<string, IEnumerable<OddsViewModel>> ActualOdds { get; set; }
  }
}
