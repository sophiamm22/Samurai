using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Entities
{
  public class MissingTeamPlayerExternalSourceAlias : BaseEntity
  {
    public int ExternalSourceID { get; set; }
    public string TeamPlayer { get; set; }
    public int TournamentID { get; set; }

    public virtual ExternalSource ExternalSource { get; set; }
    public virtual Tournament Tournament { get; set; }
  }
}
