using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class TeamsPlayer : BaseEntity
  {
    public TeamsPlayer()
    {
      this.MatchesB = new List<Match>();
      this.MatchesA = new List<Match>();
      this.TeamPlayerExternalSourceAlias = new List<TeamPlayerExternalSourceAlias>();
    }

    public string TeamName { get; set; }
    public string Slug { get; set; }
    public int? FinkTankID { get; set; }
    public virtual ICollection<Match> MatchesB { get; set; }
    public virtual ICollection<Match> MatchesA { get; set; }
    public virtual ICollection<TeamPlayerExternalSourceAlias> TeamPlayerExternalSourceAlias { get; set; }
    public override string ToString()
    {
      return TeamName;
    }
  }
}
