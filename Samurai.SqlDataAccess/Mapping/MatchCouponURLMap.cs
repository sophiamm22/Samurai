using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class MatchCouponURLMap : EntityTypeConfiguration<MatchCouponURL>
  {
    public MatchCouponURLMap()
    {
      this.Property(t => t.MatchCouponURLString).IsRequired();

      this.ToTable("MatchCouponURLs");
      this.Property(t => t.Id).HasColumnName("MatchCouponURLID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.MatchCouponURLString).HasColumnName("MatchCouponURL");

      // Relationships
      this.HasRequired(t => t.Competition)
          .WithMany(t => t.MatchCouponURLs)
          .HasForeignKey(d => d.CompetitionID);
      this.HasRequired(t => t.ExternalSource)
          .WithMany(t => t.MatchCouponURLs)
          .HasForeignKey(d => d.ExternalSourceID);

    }
  }
}
