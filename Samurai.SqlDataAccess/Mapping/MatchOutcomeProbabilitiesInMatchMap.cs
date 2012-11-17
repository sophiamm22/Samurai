using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class MatchOutcomeProbabilitiesInMatchMap : EntityTypeConfiguration<MatchOutcomeProbabilitiesInMatch>
  {
    public MatchOutcomeProbabilitiesInMatchMap()
    {
      this.ToTable("MatchOutcomeProbabilitiesInMatch");
      this.Property(t => t.Id).HasColumnName("MatchOutcomeProbabilitiesInMatchID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.MatchID).HasColumnName("MatchID_fk");
      this.Property(t => t.MatchOutcomeID).HasColumnName("MatchOutcome_fk");
      this.Property(t => t.MatchOutcomeProbability).HasPrecision(10, 9);

      // Relationships
      this.HasRequired(t => t.Match)
          .WithMany(t => t.MatchOutcomeProbabilitiesInMatches)
          .HasForeignKey(d => d.MatchID);
      this.HasRequired(t => t.MatchOutcome)
          .WithMany(t => t.MatchOutcomeProbabilitiesInMatches)
          .HasForeignKey(d => d.MatchOutcomeID);

    }
  }
}
