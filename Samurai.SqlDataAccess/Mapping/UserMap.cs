using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class UserMap : EntityTypeConfiguration<User>
  {
    public UserMap()
    {
      //Primary Key
      this.HasKey(t => t.UserID_pk);

      //Properties
      this.Property(t => t.Username).IsRequired().HasMaxLength(20);
      this.Property(t => t.Email).IsRequired().HasMaxLength(250);
      this.Property(t => t.Password).IsRequired().HasMaxLength(20);
      this.Property(t => t.PasswordSalt).IsRequired().HasMaxLength(250);
      this.Property(t => t.Comments).HasMaxLength(1000);

      // Table & Column Mappings
      this.ToTable("Users");
      this.Property(t => t.UserID_pk).HasColumnName("UserID_pk");
      this.Property(t => t.OpenID).HasColumnName("OpenID");
      this.Property(t => t.Username).HasColumnName("Username");
      this.Property(t => t.Email).HasColumnName("Email");
      this.Property(t => t.Password).HasColumnName("Password");
      this.Property(t => t.PasswordSalt).HasColumnName("PasswordSalt");
      this.Property(t => t.FirstName).HasColumnName("FirstName");
      this.Property(t => t.LastName).HasColumnName("LastName");
      this.Property(t => t.Comments).HasColumnName("Comments");
      this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
      this.Property(t => t.LastLoginDate).HasColumnName("LastLoginDate");
      this.Property(t => t.IsApproved).HasColumnName("IsApproved");
      this.Property(t => t.IsActivated).HasColumnName("IsActivated");
      this.Property(t => t.IsLockedOut).HasColumnName("IsLockedOut");
      this.Property(t => t.PasswordFailuresSinceLastSuccess).HasColumnName("PasswordFailuresSinceLastSuccess");
      this.Property(t => t.LastPasswordFailureDate).HasColumnName("LastPasswordFailureDate");
      this.Property(t => t.LastActivityDate).HasColumnName("LastActivityDate");
      this.Property(t => t.LastLockedOutDate).HasColumnName("LastLockedOutDate");
      this.Property(t => t.ConfirmationToken).HasColumnName("ConfirmationToken");
      this.Property(t => t.LastPasswordChangedDate).HasColumnName("LastPasswordChangedDate");
      this.Property(t => t.PasswordVerificationToken).HasColumnName("PasswordVerificationToken");
      this.Property(t => t.PasswordVerificationTokenExpirationDate).HasColumnName("PasswordVerificationTokenExpirationDate");



    }
  }
}
