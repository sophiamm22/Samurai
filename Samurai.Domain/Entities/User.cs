using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class User
  {
    public User()
    {
      this.Roles = new List<Role>();
    }

    public Guid UserID_pk { get; set; }
    public Guid? OpenID { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PasswordSalt { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Comments { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public bool IsApproved { get; set; }
    public bool IsActivated { get; set; }
    public bool IsLockedOut { get; set; }
    public int PasswordFailuresSinceLastSuccess { get; set; }
    public DateTime? LastPasswordFailureDate { get; set; }
    public DateTime? LastActivityDate { get; set; }
    public DateTime? LastLockedOutDate { get; set; }
    public string ConfirmationToken { get; set; }
    public DateTime? LastPasswordChangedDate { get; set; }
    public string PasswordVerificationToken { get; set; }
    public DateTime? PasswordVerificationTokenExpirationDate { get; set; }

    public virtual ICollection<Role> Roles { get; set; }

  }
}
