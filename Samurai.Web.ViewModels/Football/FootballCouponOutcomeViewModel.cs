using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Samurai.Web.ViewModels.Value;

namespace Samurai.Web.ViewModels.Football
{
  public class FootballCouponOutcomeViewModel
  {
    public string Id { get; set; }
    public string MatchIdentifier { get; set; }
    public int MatchId { get; set; }
    public string Outcome { get; set; }
    public IEnumerable<OddViewModel> OddsCollection { get; set; }

  }
}
