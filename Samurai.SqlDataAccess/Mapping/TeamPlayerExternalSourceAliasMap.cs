using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class TeamPlayerExternalSourceAliasMap : EntityTypeConfiguration<TeamPlayerExternalSourceAlias>
  {
    public TeamPlayerExternalSourceAliasMap()
    {
      this.Property(t => t.Alias).IsRequired();

      this.ToTable("TeamPlayerExternalSourceAlias");
      this.Property(t => t.Id).HasColumnName("TeamPlayerExternalSourceAliasID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.ExternalSourceID).HasColumnName("ExternalSourceID_fk");
      this.Property(t => t.TeamPlayerID).HasColumnName("TeamPlayerID_fk");

      // Relationships
      this.HasRequired(t => t.ExternalSource)
          .WithMany(t => t.TeamPlayerExternalSourceAlias)
          .HasForeignKey(d => d.ExternalSourceID);
      this.HasRequired(t => t.TeamsPlayer)
          .WithMany(t => t.TeamPlayerExternalSourceAlias)
          .HasForeignKey(d => d.TeamPlayerID);

    }
  }
}
