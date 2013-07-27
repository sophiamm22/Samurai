using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class SurfaceMap : EntityTypeConfiguration<Surface>
  {
    public SurfaceMap()
    {
      this.ToTable("Surfaces");
      this.Property(t => t.Id).HasColumnName("SurfaceID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

      this.Property(t => t.SurfaceName).IsRequired();
    }
  }
}
