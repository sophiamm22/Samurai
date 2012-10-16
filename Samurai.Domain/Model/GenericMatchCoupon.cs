using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samurai.Domain.Model
{
  public interface IGenericMatchCoupon
  {
    DateTime MatchDate { get; set; }
    string TeamOrPlayerA { get; set; }
    string TeamOrPlayerB { get; set; }
    Uri MatchURL { get; set; }
    IDictionary<Outcome, double> HeadlineOdds { get; set; }
  }

  public class GenericMatchCoupon : IGenericMatchCoupon
  {
    public GenericMatchCoupon()
    {
      HeadlineOdds = new Dictionary<Outcome, double>();
    }
    public DateTime MatchDate { get; set; }
    public string TeamOrPlayerA { get; set; }
    public string TeamOrPlayerB { get; set; }
    public Uri MatchURL { get; set; }
    public IDictionary<Outcome, double> HeadlineOdds { get; set; }
  }
}
