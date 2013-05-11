using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class TournamentExternalSourceAliasMap : EntityTypeConfiguration<TournamentExternalSourceAlias>
  {
    public TournamentExternalSourceAliasMap()
    {
      this.Property(t => t.Alias).IsRequired();

      this.ToTable("TournamentExternalSourceAlias");
      this.Property(t => t.Id).HasColumnName("TournamentExternalSourceAlias_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.ExternalSourceID).HasColumnName("ExternalSourceID_fk");
      this.Property(t => t.TournamentID).HasColumnName("TournamentID_fk");

      // Relationships
      this.HasRequired(t => t.ExternalSource)
          .WithMany(t => t.TournamentExternalSourceAlias)
          .HasForeignKey(d => d.ExternalSourceID);
      this.HasRequired(t => t.Tournament)
          .WithMany(t => t.TournamentExternalSourceAlias)
          .HasForeignKey(d => d.TournamentID);

    }
  }
}
