using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class CompetitionMap : EntityTypeConfiguration<Competition>
  {
    public CompetitionMap()
    {
      this.Property(t => t.CompetitionName).IsRequired();
      this.Property(t => t.Slug).IsRequired();

      this.ToTable("Competitions");
      this.Property(t => t.Id).HasColumnName("CompetitionID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.SportID).HasColumnName("SportID_fk");

      // Relationships
      this.HasRequired(t => t.Sport)
          .WithMany(t => t.Competitions)
          .HasForeignKey(d => d.SportID);

    }
  }
}
