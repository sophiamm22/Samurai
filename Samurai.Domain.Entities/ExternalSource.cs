using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class ExternalSource : BaseEntity
  {
    public ExternalSource()
    {
      this.CompetitionCouponURLs = new List<CompetitionCouponURL>();
      this.MatchCouponURLs = new List<MatchCouponURL>();
      this.MatchOutcomeOdds = new List<MatchOutcomeOdd>();
      this.TeamPlayerExternalSourceAlias = new List<TeamPlayerExternalSourceAlias>();
    }

    public string Source { get; set; }
    public string SourceNotes { get; set; }
    public bool OddsSource { get; set; }
    public bool TheoreticalOddsSource { get; set; }
    public string PredictionURL { get; set; }

    public virtual ICollection<CompetitionCouponURL> CompetitionCouponURLs { get; set; }
    public virtual ICollection<MatchCouponURL> MatchCouponURLs { get; set; }
    public virtual ICollection<MatchOutcomeOdd> MatchOutcomeOdds { get; set; }
    public virtual ICollection<TeamPlayerExternalSourceAlias> TeamPlayerExternalSourceAlias { get; set; }
  }
}
