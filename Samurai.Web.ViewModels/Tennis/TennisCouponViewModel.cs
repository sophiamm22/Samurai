using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels.Value;

namespace Samurai.Web.ViewModels.Tennis
{
  public class TennisCouponViewModel
  {
    public string MatchIdentifier { get; set; }
    public int MatchId { get; set; }
    public Dictionary<string, string> CouponURL { get; set; }
    public IEnumerable<OddViewModel> HomeWin { get; set; }
    public IEnumerable<OddViewModel> AwayWin { get; set; }
  }
}
