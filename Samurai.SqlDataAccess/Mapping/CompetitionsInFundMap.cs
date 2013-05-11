using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class CompetitionsInFundMap : EntityTypeConfiguration<CompetitionsInFund>
  {
    public CompetitionsInFundMap()
    {
      // Primary Key
      this.HasKey(t => t.CompetitionInFundID_pk);

      // Properties
      // Table & Column Mappings
      this.ToTable("CompetitionsInFunds");
      this.Property(t => t.CompetitionInFundID_pk).HasColumnName("CompetitionInFundID_pk");
      this.Property(t => t.FundID_fk).HasColumnName("FundID_fk");
      this.Property(t => t.CompetitionID_fk).HasColumnName("CompetitionID_fk");

      // Relationships
      this.HasRequired(t => t.Competition)
          .WithMany(t => t.CompetitionsInFunds)
          .HasForeignKey(d => d.CompetitionID_fk);
      this.HasRequired(t => t.Fund)
          .WithMany(t => t.CompetitionsInFunds)
          .HasForeignKey(d => d.FundID_fk);

    }
  }
}
