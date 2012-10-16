using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class FundMap : EntityTypeConfiguration<Fund>
  {
    public FundMap()
    {
      this.Property(t => t.FundName).IsRequired();
      this.Property(t => t.Bank).IsRequired();
      this.Property(t => t.Turnover).IsRequired();
      this.Property(t => t.Revenue).IsRequired();
      this.Property(t => t.KellyMultiplier).IsRequired();

      this.ToTable("Funds");
      this.Property(t => t.Id).HasColumnName("FundID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

      // Relationships
      this.HasMany(t => t.Competitions)
          .WithMany(t => t.Funds)
          .Map(m =>
            {
              m.ToTable("CompetitionsInFunds");
              m.MapLeftKey("FundID_fk");
              m.MapRightKey("CompetitionID_fk");
            });
    }
  }
}
