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
    public string CouponURL { get; set; }
    public IEnumerable<OddViewModel> PlayerAOdds { get; set; }
    public IEnumerable<OddViewModel> PlayerBOdds { get; set; }
  }
}
