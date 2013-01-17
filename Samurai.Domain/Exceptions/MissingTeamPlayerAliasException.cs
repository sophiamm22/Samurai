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
    public IEnumerable<MissingAlias> MissingAlias { get; private set; }

    public MissingTeamPlayerAliasException(IEnumerable<MissingAlias> missingAlias, string message)
      :base(message)
    {
      MissingAlias = missingAlias;
    }
  }

  [Serializable]
  public class MissingAlias
  {
    public string TeamOrPlayerName { get; set; }
    public string Tournament { get; set; }
    public string ExternalSource { get; set; }
  }
}
