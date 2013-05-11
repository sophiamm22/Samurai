using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class BookmakerExternalSourceAliasMap : EntityTypeConfiguration<BookmakerExternalSourceAlias>
  {
    public BookmakerExternalSourceAliasMap()
    {
      this.Property(t => t.Alias).IsRequired();
      this.ToTable("BookmakerExternalSourceAlias");
      this.Property(t => t.Id).HasColumnName("BookmakerExternalSourceAlias_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.ExternalSourceID).HasColumnName("ExternalSourceID_fk");
      this.Property(t => t.BookmakerID).HasColumnName("BookmakerID_fk");

      // Relationships
      this.HasRequired(t => t.ExternalSource)
          .WithMany(t => t.BookmakerExternalSourceAlias)
          .HasForeignKey(d => d.ExternalSourceID);
      this.HasRequired(t => t.Bookmaker)
          .WithMany(t => t.BookmakerExternalSourceAlias)
          .HasForeignKey(d => d.BookmakerID);

    }
  }
}
