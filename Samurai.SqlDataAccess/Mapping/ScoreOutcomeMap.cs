using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class ScoreOutcomeMap : EntityTypeConfiguration<ScoreOutcome>
  {
    public ScoreOutcomeMap()
    {
      this.ToTable("ScoreOutcomes");
      this.Property(t => t.Id).HasColumnName("ScoreOutcomeID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.MatchOutcomeID).HasColumnName("MatchOutcomeID_fk");
      this.Property(t => t.TeamAScore).HasColumnName("TeamAScore");
      this.Property(t => t.TeamBScore).HasColumnName("TeamBScore");

      // Relationships
      this.HasRequired(t => t.MatchOutcome)
          .WithMany(t => t.ScoreOutcomes)
          .HasForeignKey(d => d.MatchOutcomeID);

    }
  }
}
