using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Entities
{
  public class TournamentEvent : BaseEntity
  {
    public TournamentEvent()
    {
      this.Matches = new List<Match>();
    }

    public int TournamentID { get; set; }
    public int? SurfaceID { get; set; }
    public string EventName { get; set; }
    public string Slug { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool TournamentInProgress { get; set; }
    public bool TournamentCompleted { get; set; }

    public virtual Tournament Tournament { get; set; }
    public virtual Surface Surface { get; set; }

    public virtual ICollection<Match> Matches { get; set; }
  }
}
