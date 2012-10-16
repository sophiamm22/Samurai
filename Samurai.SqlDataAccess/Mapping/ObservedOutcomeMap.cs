using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class ObservedOutcomeMap : EntityTypeConfiguration<ObservedOutcome>
  {
    public ObservedOutcomeMap()
    {
      this.ToTable("ObservedOutcomes");
      this.Property(t => t.Id).HasColumnName("ObservedOutcomeID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.MatchID).HasColumnName("MatchID_fk");
      this.Property(t => t.ScoreOutcomeID).HasColumnName("ScoreOutcomeID_fk");

      // Relationships
      this.HasRequired(t => t.Match)
          .WithMany(t => t.ObservedOutcomes)
          .HasForeignKey(d => d.MatchID);
      this.HasRequired(t => t.ScoreOutcome)
          .WithMany(t => t.ObservedOutcomes)
          .HasForeignKey(d => d.ScoreOutcomeID);

    }
  }
}
