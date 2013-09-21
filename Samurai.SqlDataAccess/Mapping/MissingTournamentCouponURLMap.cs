using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class MissingTournamentCouponURLMap : EntityTypeConfiguration<MissingTournamentCouponURL>
  {
    public MissingTournamentCouponURLMap()
    {
      this.ToTable("MissingTournamentCouponURL");

      this.Property(t => t.Id).HasColumnName("MissingTournamentCouponURL_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.ExternalSourceID).HasColumnName("ExternalSourceID_fk");
      this.Property(t => t.TournamentID).HasColumnName("TournamentID_fk");

      // Relationships
      this.HasRequired(t => t.ExternalSource)
          .WithMany(t => t.MissingTournamentCouponURLs)
          .HasForeignKey(d => d.ExternalSourceID);
      this.HasRequired(t => t.Tournament)
          .WithMany(t => t.MissingTournamentCouponURLs)
          .HasForeignKey(d => d.TournamentID);
    }
  }
}
