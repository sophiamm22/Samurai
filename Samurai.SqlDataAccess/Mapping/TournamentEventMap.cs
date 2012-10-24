using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class TournamentEventMap : EntityTypeConfiguration<TournamentEvent>
  {
    public TournamentEventMap()
    {
      this.Property(t => t.EventName).IsRequired();
      this.Property(t => t.Slug).IsRequired();
      this.Property(t => t.StartDate).IsRequired();

      this.ToTable("TournamentEvents");
      this.Property(t => t.Id).HasColumnName("TournamentEventID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.TournamentID).HasColumnName("TournamentID_fk");

      //Relationships
      this.HasRequired(t => t.Tournament)
          .WithMany(t => t.TournamentEvents)
          .HasForeignKey(d => d.TournamentID);
    }
  }
}
