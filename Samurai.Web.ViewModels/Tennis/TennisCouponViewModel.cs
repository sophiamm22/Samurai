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
    private string _matchIdentifier;
    public string MatchIdentifier 
    {
      get { return _matchIdentifier; }
      set 
      { 
        _matchIdentifier = value; 
      }
    }
    public Dictionary<string, string> CouponURL { get; set; }
    public IEnumerable<OddViewModel> PlayerAOdds { get; set; }
    public IEnumerable<OddViewModel> PlayerBOdds { get; set; }
  }
}
