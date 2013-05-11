using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class TournamentCouponURLMap : EntityTypeConfiguration<TournamentCouponURL>
  {
    public TournamentCouponURLMap()
    {
      this.Property(t => t.CouponURL).IsRequired();

      this.ToTable("TournamentCouponURLs");
      this.Property(t => t.Id).HasColumnName("TournamentCouponURLID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.TournamentID).HasColumnName("TournamentID_fk");
      this.Property(t => t.ExternalSourceID).HasColumnName("ExternalSourceID_fk");

      // Relationships
      this.HasRequired(t => t.Tournament)
          .WithMany(t => t.TournamentCouponURLs)
          .HasForeignKey(d => d.TournamentID);
      this.HasRequired(t => t.ExternalSource)
          .WithMany(t => t.TournamentCouponURLs)
          .HasForeignKey(d => d.ExternalSourceID);

    }
  }
}
