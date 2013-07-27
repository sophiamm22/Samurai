using System;
using System.Collections.Generic;


namespace Samurai.Domain.Entities
{
  public class Surface : BaseEntity
  {
    public Surface()
    {
      this.TournamentEvents = new List<TournamentEvent>();
    }

    public string SurfaceName { get; set; }

    public virtual ICollection<TournamentEvent> TournamentEvents { get; set; }

  }
}
