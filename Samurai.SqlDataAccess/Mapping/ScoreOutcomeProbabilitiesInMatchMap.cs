using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class ScoreOutcomeProbabilitiesInMatchMap : EntityTypeConfiguration<ScoreOutcomeProbabilitiesInMatch>
  {
    public ScoreOutcomeProbabilitiesInMatchMap()
    {
      this.ToTable("ScoreOutcomeProbabilitiesInMatch");
      this.Property(t => t.Id).HasColumnName("ScoreOutcomeProbabilitiesInMatchID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.MatchID).HasColumnName("MatchID_fk");
      this.Property(t => t.ScoreOutcomeID).HasColumnName("ScoreOutcomeID_fk");
      this.Property(t => t.ScoreOutcomeProbability).HasPrecision(10, 9);

      // Relationships
      this.HasRequired(t => t.Match)
          .WithMany(t => t.ScoreOutcomeProbabilitiesInMatches)
          .HasForeignKey(d => d.MatchID);
      this.HasRequired(t => t.ScoreOutcome)
          .WithMany(t => t.ScoreOutcomeProbabilitiesInMatches)
          .HasForeignKey(d => d.ScoreOutcomeID);

    }
  }
}
