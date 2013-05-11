using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samurai.Domain.Model
{
  public interface IGenericTournamentCoupon
  {
    string TournamentName { get; set; }
    Uri TournamentURL { get; set; }
    IEnumerable<GenericMatchCoupon> Matches { get; set; }
  }

  public class GenericTournamentCoupon : IGenericTournamentCoupon
  {
    public GenericTournamentCoupon()
    {
      Matches = new List<GenericMatchCoupon>();
    }
    public string TournamentName { get; set; }
    public Uri TournamentURL { get; set; }
    public IEnumerable<GenericMatchCoupon> Matches { get; set; }
  }

}
