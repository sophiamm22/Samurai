using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class Competition : BaseEntity
  {
    public Competition()
    {
      this.Funds = new List<Fund>();
      this.Tournaments = new List<Tournament>();
      this.CompetitionCouponURLs = new List<CompetitionCouponURL>();
    }

    public int SportID { get; set; }
    public string CompetitionName { get; set; }
    public string Slug { get; set; }
    public decimal EdgeRequired { get; set; }
    public int? GamesRequiredForBet { get; set; }

    public virtual Sport Sport { get; set; }
    public virtual ICollection<Fund> Funds { get; set; }
    public virtual ICollection<Tournament> Tournaments { get; set; }
    public virtual ICollection<CompetitionCouponURL> CompetitionCouponURLs { get; set; }
  }
}
