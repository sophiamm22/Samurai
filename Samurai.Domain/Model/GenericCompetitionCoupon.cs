using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samurai.Domain.Model
{
  public interface IGenericCompetitionCoupon
  {
    string CompetitionName { get; set; }
    Uri CompetitionURL { get; set; }
    IEnumerable<IGenericMatchCoupon> Matches { get; set; }
  }

  public class GenericCompetitionCoupon : IGenericCompetitionCoupon
  {
    public GenericCompetitionCoupon()
    {
      Matches = new List<IGenericMatchCoupon>();
    }
    public string CompetitionName { get; set; }
    public Uri CompetitionURL { get; set; }
    public IEnumerable<IGenericMatchCoupon> Matches { get; set; }
  }

}
