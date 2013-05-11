using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class Sport : BaseEntity
  {
    public Sport()
    {
      this.Competitions = new List<Competition>();
    }

    //public int SportID_pk { get; set; }
    public string SportName { get; set; }
    public virtual ICollection<Competition> Competitions { get; set; }
  }
}
