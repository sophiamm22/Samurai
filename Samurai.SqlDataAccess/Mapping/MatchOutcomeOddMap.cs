using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class MatchOutcomeOddMap : EntityTypeConfiguration<MatchOutcomeOdd>
  {
    public MatchOutcomeOddMap()
    {
      this.ToTable("MatchOutcomeOdds");
      this.Property(t => t.Id).HasColumnName("MatchOutcomeOddID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.ExternalSourceID).HasColumnName("ExternalSourceID_fk");
      this.Property(t => t.MatchOutcomeProbabilitiesInMatchID).HasColumnName("MatchOutcomeProbabilitiesInMatchID_fk");
      this.Property(t => t.BookmakerID).HasColumnName("BookmakerID_fk");

      // Relationships
      this.HasRequired(t => t.Bookmaker)
          .WithMany(t => t.MatchOutcomeOdds)
          .HasForeignKey(d => d.BookmakerID);
      this.HasRequired(t => t.ExternalSource)
          .WithMany(t => t.MatchOutcomeOdds)
          .HasForeignKey(d => d.ExternalSourceID);
      this.HasRequired(t => t.MatchOutcomeProbabilitiesInMatch)
          .WithMany(t => t.MatchOutcomeOdds)
          .HasForeignKey(d => d.MatchOutcomeProbabilitiesInMatchID);

    }
  }
}
