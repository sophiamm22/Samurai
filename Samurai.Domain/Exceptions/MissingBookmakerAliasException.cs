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
    public IEnumerable<MissingBookmakerAliasObject> MissingAlias { get; private set; }

    public MissingBookmakerAliasException(IEnumerable<MissingBookmakerAliasObject> missingAlias, string message)
      : base(message)
    {
      MissingAlias = missingAlias;
    }
  }

  [Serializable]
  public class MissingBookmakerAliasObject
  {
    public string Bookmaker { get; set; }
    public int BookmakerID { get; set; }
    public string ExternalSource { get; set; }
    public int ExternalSourceID { get; set; }

    public override string ToString()
    {
      return string.Format("{0} @ {1}", Bookmaker, ExternalSource);
    }
  }
}
