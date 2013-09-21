using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Entities
{
  public class MissingBookmakerExternalSourceAlias : BaseEntity
  {
    public int ExternalSourceID { get; set; }
    public string Bookmaker { get; set; }

    public virtual ExternalSource ExternalSource { get; set; }
  }
}
