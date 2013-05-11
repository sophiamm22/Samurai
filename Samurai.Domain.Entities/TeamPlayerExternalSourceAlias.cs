using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class TeamPlayerExternalSourceAlias : BaseEntity
  {
    public int ExternalSourceID { get; set; }
    public int TeamPlayerID { get; set; }
    public string Alias { get; set; }

    public virtual ExternalSource ExternalSource { get; set; }
    public virtual TeamPlayer TeamsPlayer { get; set; }
  }
}
