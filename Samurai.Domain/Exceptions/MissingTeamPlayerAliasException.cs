using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Exceptions
{
  [Serializable]
  public class MissingTeamPlayerAliasException : Exception
  {
    public IEnumerable<MissingTeamPlayerAliasObject> MissingAlias { get; private set; }

    public MissingTeamPlayerAliasException(IEnumerable<MissingTeamPlayerAliasObject> missingAlias, string message)
      :base(message)
    {
      MissingAlias = missingAlias;
    }
  }

  [Serializable]
  public class MissingTeamPlayerAliasObject
  {
    public string TeamOrPlayerName { get; set; }

    public string Tournament { get; set; }
    public int TournamentID { get; set; }
    public string ExternalSource { get; set; }
    public int ExternalSourceID { get; set; }

    public override string ToString()
    {
      return string.Format("{0} @ {1} from {2}", TeamOrPlayerName, Tournament, ExternalSource);
    }

  }
}
