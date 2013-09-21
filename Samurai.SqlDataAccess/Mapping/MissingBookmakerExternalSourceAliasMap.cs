using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class MissingBookmakerExternalSourceAliasMap : EntityTypeConfiguration<MissingBookmakerExternalSourceAlias>
  {
    public MissingBookmakerExternalSourceAliasMap()
    {
      this.ToTable("MissingBookmakerExternalSourceAlias");
      
      this.Property(t => t.Bookmaker).IsRequired();

      this.Property(t => t.Id).HasColumnName("MissingBookmakerExternalSourceAliasID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.ExternalSourceID).HasColumnName("ExternalSourceID_fk");
      
      // Relationships
      this.HasRequired(t => t.ExternalSource)
          .WithMany(t => t.MissingBookmakerExternalSourceAlias)
          .HasForeignKey(d => d.ExternalSourceID);
    }
  }
}
