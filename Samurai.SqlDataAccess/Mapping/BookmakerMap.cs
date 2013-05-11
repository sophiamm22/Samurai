using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class BookmakerMap : EntityTypeConfiguration<Bookmaker>
  {
    public BookmakerMap()
    {
      this.Property(t => t.BookmakerName).IsRequired();
      this.Property(t => t.Slug).IsRequired();
      this.Property(t => t.BookmakerURL).IsRequired();
      this.Property(t => t.OddsCheckerShortID).HasMaxLength(2);

      this.ToTable("Bookmakers");
      this.Property(t => t.Id).HasColumnName("BookmakerID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.OddsCheckerShortID).HasColumnName("OddsCheckerShortID_efk");
    }
  }
}
