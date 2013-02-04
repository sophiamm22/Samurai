using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Exceptions
{
  [Serializable]
  public class MissingBookmakerAliasException : Exception
  {
    public IEnumerable<MissingBookmakerAlias> MissingAlias { get; private set; }

    public MissingBookmakerAliasException(IEnumerable<MissingBookmakerAlias> missingAlias, string message)
      : base(message)
    {
      MissingAlias = missingAlias;
    }
  }

  [Serializable]
  public class MissingBookmakerAlias
  {
    public string Bookmaker { get; set; }
    public string ExternalSource { get; set; }

    public override string ToString()
    {
      return string.Format("{0} @ {1}", Bookmaker, ExternalSource);
    }
  }
}
