using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Web.ViewModels.Tennis
{
  public class TennisFixtureViewModel
  {
    public string MatchIdentifier { get; set; }
    
    public string Tournament { get; set; }
    public int Year { get; set; }
    public DateTime MatchDate { get; set; }
    public string PlayerAFirstName { get; set; }
    public string PlayerASurname { get; set; }
    public string PlayerBFirstName { get; set; }
    public string PlayerBSurname { get; set; }

    public string ScoreLine { get; set; }
    public string Comment { get; set; }

    public TennisPredictionViewModel Predictions { get; set; }
    public TennisCouponViewModel Coupons { get; set; }
  }
}
