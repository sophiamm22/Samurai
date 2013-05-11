using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class Match : BaseEntity
  {
    public Match()
    {
      //this.BettingPAndLs = new List<BettingPAndL>();
      this.MatchOutcomeProbabilitiesInMatches = new List<MatchOutcomeProbabilitiesInMatch>();
      this.ObservedOutcomes = new List<ObservedOutcome>();
      this.ScoreOutcomeProbabilitiesInMatches = new List<ScoreOutcomeProbabilitiesInMatch>();
    }

    //public int MatchID_pk { get; set; }
    public int CompetitionID { get; set; }
    public DateTime MatchDate { get; set; }
    public string Venue { get; set; }
    public int TeamAID { get; set; }
    public int TeamBID { get; set; }
    public bool EligibleForBetting { get; set; }
    public int? IKTSGameWeek { get; set; }
    //public virtual ICollection<BettingPAndL> BettingPAndLs { get; set; }
    public virtual Competition Competition { get; set; }
    public virtual TeamsPlayer TeamsPlayerB { get; set; }
    public virtual TeamsPlayer TeamsPlayerA { get; set; }
    public virtual ICollection<MatchOutcomeProbabilitiesInMatch> MatchOutcomeProbabilitiesInMatches { get; set; }
    public virtual ICollection<ObservedOutcome> ObservedOutcomes { get; set; }
    public virtual ICollection<ScoreOutcomeProbabilitiesInMatch> ScoreOutcomeProbabilitiesInMatches { get; set; }
  }
}
