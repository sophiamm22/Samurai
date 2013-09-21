using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class MissingTeamPlayerExternalSourceAliasMap : EntityTypeConfiguration<MissingTeamPlayerExternalSourceAlias>
  {
    public MissingTeamPlayerExternalSourceAliasMap()
    {
      this.ToTable("MissingTeamPlayerExternalSourceAlias");

      this.Property(t => t.TeamPlayer).IsRequired();

      this.Property(t => t.Id).HasColumnName("MissingTeamPlayerExternalSourceAlias_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.ExternalSourceID).HasColumnName("ExternalSourceID_fk");
      this.Property(t => t.TournamentID).HasColumnName("TournamentID_fk");

      // Relationships
      this.HasRequired(t => t.ExternalSource)
          .WithMany(t => t.MissingTeamPlayerExternalSourceAlias)
          .HasForeignKey(d => d.ExternalSourceID);
      this.HasRequired(t => t.Tournament)
          .WithMany(t => t.MissingTeamPlayerExternalSourceAlias)
          .HasForeignKey(d => d.TournamentID);
    }
  }
}
