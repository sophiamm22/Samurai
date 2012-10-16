using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class CompetitionCouponURLMap : EntityTypeConfiguration<CompetitionCouponURL>
  {
    public CompetitionCouponURLMap()
    {
      this.Property(t => t.CouponURL).IsRequired();

      this.ToTable("CompetitionCouponURLs");
      this.Property(t => t.Id).HasColumnName("CompetitionCouponURLID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.CompetitionID).HasColumnName("CompetitionID_fk");
      this.Property(t => t.ExternalSourceID).HasColumnName("ExternalSourceID_fk");

      // Relationships
      this.HasRequired(t => t.Competition)
          .WithMany(t => t.CompetitionCouponURLs)
          .HasForeignKey(d => d.CompetitionID);
      this.HasRequired(t => t.ExternalSource)
          .WithMany(t => t.CompetitionCouponURLs)
          .HasForeignKey(d => d.ExternalSourceID);

    }
  }
}
