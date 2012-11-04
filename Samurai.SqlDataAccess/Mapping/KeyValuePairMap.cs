using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class KeyValuePairMap : EntityTypeConfiguration<KeyValuePair>
  {
    public KeyValuePairMap()
    {
      this.ToTable("KeyValuePairs");
      this.Property(t => t.Id).HasColumnName("KeyValuePair_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
    }
  }
}
