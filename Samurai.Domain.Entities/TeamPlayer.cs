using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class TeamPlayer : BaseEntity
  {
    public TeamPlayer()
    {
      this.MatchesB = new List<Match>();
      this.MatchesA = new List<Match>();
      this.TeamPlayerExternalSourceAlias = new List<TeamPlayerExternalSourceAlias>();
    }

    public string Name { get; set; }
    public string FirstName { get; set; }
    public string Slug { get; set; }
    public string ExternalID { get; set; }
    public virtual ICollection<Match> MatchesB { get; set; }
    public virtual ICollection<Match> MatchesA { get; set; }
    public virtual ICollection<TeamPlayerExternalSourceAlias> TeamPlayerExternalSourceAlias { get; set; }
    public virtual ICollection<MissingTeamPlayerExternalSourceAlias> MissingTeamPlayerExternalSourceAlias { get; set; }

    public override string ToString()
    {
      return Name;
    }
  }
}
