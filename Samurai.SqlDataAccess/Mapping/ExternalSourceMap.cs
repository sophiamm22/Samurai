using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class ExternalSourceMap : EntityTypeConfiguration<ExternalSource>
  {
    public ExternalSourceMap()
    {
      this.Property(t => t.Source).IsRequired();

      this.Property(t => t.Id).HasColumnName("ExternalSourceID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
    }
  }
}
