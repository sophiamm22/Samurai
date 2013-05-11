using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class TournamentMap : EntityTypeConfiguration<Tournament>
  {
    public TournamentMap()
    {
      this.Property(t => t.TournamentName).IsRequired();
      this.Property(t => t.Slug).IsRequired();

      this.ToTable("Tournaments");
      this.Property(t => t.Id).HasColumnName("TournamentID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

      //Relationships
      this.HasRequired(t => t.Competition)
          .WithMany(t => t.Tournaments)
          .HasForeignKey(d => d.CompetitionID);
    }
  }
}
