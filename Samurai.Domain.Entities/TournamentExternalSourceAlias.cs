using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Entities
{
  public class TournamentExternalSourceAlias : BaseEntity
  {
    public int ExternalSourceID { get; set; }
    public int TournamentID { get; set; }
    public string Alias { get; set; }

    public virtual ExternalSource ExternalSource { get; set; }
    public virtual Tournament Tournament { get; set; }
  }
}
