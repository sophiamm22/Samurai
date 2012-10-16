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

    public int? FinkTankID { get; set; }
    public string GuardianID { get; set; }
    public string OddscheckerID { get; set; }
    public string OddsPortalID { get; set; }
    public string TeamDisplayName { get; set; }
    public virtual ICollection<Match> MatchesB { get; set; }
    public virtual ICollection<Match> MatchesA { get; set; }
    public virtual ICollection<TeamPlayerExternalSourceAlias> TeamPlayerExternalSourceAlias { get; set; }
  }
}
