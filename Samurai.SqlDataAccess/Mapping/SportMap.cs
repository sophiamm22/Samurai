using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class SportMap : EntityTypeConfiguration<Sport>
  {
    public SportMap()
    {
      this.Property(t => t.SportName).IsRequired();

      this.ToTable("Sports");
      this.Property(t => t.Id).HasColumnName("SportID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
    }
  }
}
