using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class MatchOutcomeMap : EntityTypeConfiguration<MatchOutcome>
  {
    public MatchOutcomeMap()
    {
      this.Property(t => t.MatchOutcomeString).IsRequired().HasMaxLength(50);

      this.ToTable("MatchOutcomes");
      this.Property(t => t.Id).HasColumnName("MatchOutcomeID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.MatchOutcomeString).HasColumnName("MatchOutcome");
    }
  }
}
