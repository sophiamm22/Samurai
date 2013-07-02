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
    public IEnumerable<MissingTournamentCouponURLObject> MissingData { get; private set; }

    public TournamentCouponURLMissingException(IEnumerable<MissingTournamentCouponURLObject> missingData, string message)
      : base(message)
    {
      MissingData = missingData;
    }
  }

  [Serializable]
  public class MissingTournamentCouponURLObject
  {
    public string ExternalSource { get; set; }
    public int ExternalSourceID { get; set; }
    public string Tournament { get; set; }
    public int TournamentID { get; set; }
  }
}
