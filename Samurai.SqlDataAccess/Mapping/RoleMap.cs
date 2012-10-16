using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class RoleMap : EntityTypeConfiguration<Role>
  {
    public RoleMap()
    {
      // Primary Key
      this.HasKey(x => x.RoleID_pk);

      this.Property(t => t.RoleName).IsRequired().HasMaxLength(100);

      this.ToTable("Roles");
      this.Property(t => t.RoleID_pk).HasColumnName("RoleID_pk");
      this.Property(t => t.RoleName).HasColumnName("RoleName");
      this.Property(t => t.Description).HasColumnName("Description");

      this.HasMany(t => t.Users)
          .WithMany(t => t.Roles)
          .Map(m =>
            {
              m.ToTable("UsersInRoles");
              m.MapLeftKey("RoleID_fk");
              m.MapRightKey("UserID_fk");
            });

    }
  }
}
