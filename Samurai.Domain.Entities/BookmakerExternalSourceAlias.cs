using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Entities
{
  public class BookmakerExternalSourceAlias : BaseEntity
  {
    public int ExternalSourceID { get; set; }
    public int BookmakerID { get; set; }
    public string Alias { get; set; }

    public virtual ExternalSource ExternalSource { get; set; }
    public virtual Bookmaker Bookmaker { get; set; }
  }
}
