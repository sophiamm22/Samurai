using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Exceptions
{
  [Serializable]
  public class TournamentCouponURLMissingException : Exception
  {
    public IEnumerable<MissingTournamentCouponURL> MissingData { get; private set; }

    public TournamentCouponURLMissingException(IEnumerable<MissingTournamentCouponURL> missingData, string message)
      : base(message)
    {
      MissingData = missingData;
    }
  }

  [Serializable]
  public class MissingTournamentCouponURL
  {
    public string ExternalSource { get; set; }
    public string Tournament { get; set; }
  }
}
