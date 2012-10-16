using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class Competition : BaseEntity
  {
    public Competition()
    {
      this.CompetitionCouponURLs = new List<CompetitionCouponURL>();
      this.MatchCouponURLs = new List<MatchCouponURL>();
      this.Matches = new List<Match>();
      this.Funds = new List<Fund>();
    }

    //public int CompetitionID_pk { get; set; }
    public int SportID { get; set; }
    public string CompetitionName { get; set; }
    public decimal OverroundRequired { get; set; }
    public Nullable<int> GamesRequiredForBet { get; set; }

    public virtual ICollection<CompetitionCouponURL> CompetitionCouponURLs { get; set; }
    public virtual Sport Sport { get; set; }
    public virtual ICollection<MatchCouponURL> MatchCouponURLs { get; set; }
    public virtual ICollection<Match> Matches { get; set; }
    public virtual ICollection<Fund> Funds { get; set; }
  }
}
