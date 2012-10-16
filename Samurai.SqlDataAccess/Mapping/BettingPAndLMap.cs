using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class BettingPAndLMap : EntityTypeConfiguration<BettingPAndL>
  {
    public BettingPAndLMap()
    {
      this.ToTable("BettingPAndL");
      this.Property(t => t.Id).HasColumnName("BettingPAndLID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.MatchOutcomeOddID).HasColumnName("MatchOutcomeOddID_fk");

      this.HasRequired(t => t.MatchOutcomeOdd)
          .WithMany(t => t.BettingPAndLs)
          .HasForeignKey(d => d.MatchOutcomeOddID);

    }
  }
}
