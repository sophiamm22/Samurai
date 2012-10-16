using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class UserProfileMap : EntityTypeConfiguration<UserProfile>
  {
    public UserProfileMap()
    {
      this.ToTable("UserProfiles");
      this.Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
    }
  }
}
