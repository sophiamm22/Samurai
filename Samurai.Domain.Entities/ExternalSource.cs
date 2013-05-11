using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class ExternalSource : BaseEntity
  {
    public ExternalSource()
    {
      this.CompeitionCouponURLs = new List<CompetitionCouponURL>();
      this.TournamentCouponURLs = new List<TournamentCouponURL>();
      this.MatchCouponURLs = new List<MatchCouponURL>();
      this.MatchOutcomeOdds = new List<MatchOutcomeOdd>();
      this.TeamPlayerExternalSourceAlias = new List<TeamPlayerExternalSourceAlias>();
      this.TournamentExternalSourceAlias = new List<TournamentExternalSourceAlias>();
      this.BookmakerExternalSourceAlias = new List<BookmakerExternalSourceAlias>();
    }

    public string Source { get; set; }
    public string SourceNotes { get; set; }
    public bool OddsSource { get; set; }
    public bool TheoreticalOddsSource { get; set; }
    public string PredictionURL { get; set; }
    public bool UseByDefault { get; set; }
    public bool PrescreenDecider { get; set; }

    public virtual ICollection<CompetitionCouponURL> CompeitionCouponURLs { get; set; }
    public virtual ICollection<TournamentCouponURL> TournamentCouponURLs { get; set; }
    public virtual ICollection<MatchCouponURL> MatchCouponURLs { get; set; }
    public virtual ICollection<MatchOutcomeOdd> MatchOutcomeOdds { get; set; }
    public virtual ICollection<TeamPlayerExternalSourceAlias> TeamPlayerExternalSourceAlias { get; set; }
    public virtual ICollection<TournamentExternalSourceAlias> TournamentExternalSourceAlias { get; set; }
    public virtual ICollection<BookmakerExternalSourceAlias> BookmakerExternalSourceAlias { get; set; }
  }
}
