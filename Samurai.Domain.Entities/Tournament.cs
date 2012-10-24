using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Entities
{
  public class Tournament : BaseEntity
  {
    public Tournament()
    {
      this.TournamentExternalSourceAlias = new List<TournamentExternalSourceAlias>();
      this.TournamentEvents = new List<TournamentEvent>();
    }

    public int CompetitionID { get; set; }
    public string TournamentName { get; set; }
    public string Slug { get; set; }
    public string Location { get; set; }

    public virtual Competition Competition { get; set; }
    public virtual ICollection<TournamentExternalSourceAlias> TournamentExternalSourceAlias { get; set; }
    public virtual ICollection<TournamentEvent> TournamentEvents { get; set; }

  }
}
