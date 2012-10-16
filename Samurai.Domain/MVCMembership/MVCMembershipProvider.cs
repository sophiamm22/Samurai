using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Security;
using System.Configuration;
using System.Collections.Specialized;

using Samurai.Domain.Repository;
using Samurai.Domain.Entities;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.Domain.MVCMembership
{
  //deprecated in favour of simple membership, dependencies were really hard to provide so I went for a simple membership provider
  public class MVCMembershipProvider : MembershipProvider
  {
    private IMVCMembershipRepository mvcMembershipService;

    public override void Initialize(string name, NameValueCollection config)
    {
      base.Initialize(name, config);
      //Dependency Injection in .NET, Mark Seemann, page 228
      this.mvcMembershipService = null; //see above, dependencies were a ball-ache so went for simple membership
      //var container = (IAccountContainer)HttpContext.Current.Application["container"];
      //this.mvcMembershipService = container.ResolveMembershipService();
    }

    public override string ApplicationName
    {
      get
      { return this.GetType().Assembly.GetName().Name.ToString(); }
      set
      { this.ApplicationName = this.GetType().Assembly.GetName().Name.ToString(); }
    }

    public override int MaxInvalidPasswordAttempts
    {
      get { return 5; }
    }

    public override int MinRequiredNonAlphanumericCharacters
    {
      get { return 0; }
    }

    public override int MinRequiredPasswordLength
    {
      get { return 6; }
    }

    public override int PasswordAttemptWindow
    {
      get { return 0; }
    }

    public override MembershipPasswordFormat PasswordFormat
    {
      get { return MembershipPasswordFormat.Hashed; }
    }

    public override string PasswordStrengthRegularExpression
    {
      get { return String.Empty; }
    }

    public override bool RequiresUniqueEmail
    {
      get { return true; }
    }


    public override MembershipUser CreateUser(string username, string password,
      string email, string passwordQuestion, string passwordAnswer, bool isApproved,
      object providerUserKey, out MembershipCreateStatus status)
    {
      var args = new ValidatePasswordEventArgs(username, password, true);
      OnValidatingPassword(args);
      if (args.Cancel)
      {
        status = MembershipCreateStatus.InvalidPassword;
        return null;
      }
      if (RequiresUniqueEmail && GetUserNameByEmail(email) != string.Empty)
      {
        status = MembershipCreateStatus.DuplicateEmail;
        return null;
      }
      if (string.IsNullOrEmpty(username))
      {
        status = MembershipCreateStatus.InvalidUserName;
        return null;
      }
      if (string.IsNullOrEmpty(password))
      {
        status = MembershipCreateStatus.InvalidPassword;
        return null;
      }
      if (RequiresUniqueEmail && string.IsNullOrEmpty(email))
      {
        status = MembershipCreateStatus.InvalidEmail;
        return null;
      }
      if (RequiresUniqueEmail && mvcMembershipService.GetUserNameByEmail(email) != string.Empty)
      {
        status = MembershipCreateStatus.DuplicateEmail;
        return null;
      }

      var userByOpenID = GetUser(providerUserKey, true);
      var userByUsername = GetUser(username, false);

      if (userByOpenID != null || userByUsername != null)
      {
        status = MembershipCreateStatus.DuplicateUserName;
        return null;
      }

      var userSalt = mvcMembershipService.CreateSalt();
      var hashedPassword = mvcMembershipService.CreatePasswordHash(password, userSalt);

      var newUser = new User
      {
        UserID_pk = Guid.NewGuid(),
        OpenID = (Guid)providerUserKey,
        Username = username,
        Email = email,
        Password = hashedPassword,
        PasswordSalt = userSalt,
        CreatedDate = DateTime.Now,
        IsActivated = false,
        IsApproved = false,
        IsLockedOut = false,
        LastLockedOutDate = DateTime.Now,
        LastActivityDate = DateTime.Now,
        LastLoginDate = DateTime.Now
      };

      mvcMembershipService.AddUser(newUser);

      status = MembershipCreateStatus.Success;
      return GetUser(username, true);
    }

    public override bool ValidateUser(string username, string password)
    {
      if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        return false;
      var user = mvcMembershipService.GetUserByUserName(username);
      if (user == null)
        return false;
      if (user.Password == mvcMembershipService.CreatePasswordHash(password, user.PasswordSalt))
        return true;
      else
        return false;
    }

    public override MembershipUser GetUser(string username, bool userIsOnline)
    {
      var user = mvcMembershipService.GetUserByUserName(username);
      if (user != null)
        return UserTouched(user);
      else
        return null;
    }

    public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
    {
      var user = mvcMembershipService.GetUserByOpenId((Guid)providerUserKey);
      if (user != null)
        return UserTouched(user);
      else
        return null;
    }

    public MembershipUser GetUser(Guid userId, bool userIsOnline)
    {
      var user = mvcMembershipService.GetUserByUserId(userId);
      return UserTouched(user);
    }

    public override bool ChangePassword(string username, string oldPassword, string newPassword)
    {
      if (!ValidateUser(username, oldPassword))
        return false;
      var args = new ValidatePasswordEventArgs(username, newPassword, true);
      OnValidatingPassword(args);

      if (args.Cancel)
      {
        if (args.FailureInformation != null)
          throw args.FailureInformation;
        else
          throw new MembershipPasswordException("Change password cancelled due to new password validation failure.");
      }

      var user = mvcMembershipService.GetUserByUserName(username);
      if (user == null) return false;
      var userSalt = mvcMembershipService.CreateSalt();
      var hashedPassword = mvcMembershipService.CreatePasswordHash(newPassword, userSalt);
      user.Password = hashedPassword;
      user.PasswordSalt = userSalt;
      mvcMembershipService.SaveChanges();
      return true;
    }

    public override bool UnlockUser(string userName)
    {
      var userToUnlock = mvcMembershipService.GetUserByUserName(userName);
      if (userToUnlock.IsLockedOut)
      {
        userToUnlock.IsLockedOut = true;
        mvcMembershipService.SaveChanges();
        return true;
      }
      return false;
    }

    public override int GetNumberOfUsersOnline()
    {
      var since = DateTime.UtcNow.Subtract(new TimeSpan(0, Membership.UserIsOnlineTimeWindow, 0));
      return mvcMembershipService.GetNumberOfUsersOnline(since);
    }

    public override bool DeleteUser(string username, bool deleteAllRelatedData)
    {
      if (string.IsNullOrEmpty(username))
        return false;
      var user = mvcMembershipService.GetUserByUserName(username);
      if (user == null)
        return false;
      try
      {
        mvcMembershipService.DeleteUser(user);
        return true;
      }
      catch
      {
        return false;
      }
    }

    public override string GetUserNameByEmail(string email)
    {
      var result = mvcMembershipService.GetUserNameByEmail(email);
      return string.IsNullOrEmpty(result) ? string.Empty : result;
    }

    public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      var users = mvcMembershipService.GetUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords);
      var membershipUsers = new MembershipUserCollection();
      foreach (var user in users)
        membershipUsers.Add(UserToMembershipUser(user));
      
      return membershipUsers;
    }

    public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      var users = mvcMembershipService.GetUsersByUserName(usernameToMatch, pageIndex, pageSize, out totalRecords);
      var membershipUsers = new MembershipUserCollection();
      foreach (var user in users)
        membershipUsers.Add(UserToMembershipUser(user));

      return membershipUsers;
    }

    public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
    {
      var users = mvcMembershipService.GetAllUsers(pageIndex, pageSize, out totalRecords);
      var membershipUsers = new MembershipUserCollection();
      foreach (var user in users)
        membershipUsers.Add(UserToMembershipUser(user));

      return membershipUsers;
    }

    private MembershipUser UserTouched(User user)
    {
      mvcMembershipService.UserTouched(user);
      return UserToMembershipUser(user);
    }

    private MembershipUser UserToMembershipUser(User user)
    {
      return new MembershipUser(Membership.Provider.Name, user.Username, user.OpenID,
        user.Email, user.UserID_pk.ToString(), null, true, false, user.CreatedDate, 
        user.LastLoginDate ?? new DateTime(1900, 01, 01), user.LastActivityDate ?? new DateTime(1900, 01, 01),
        new DateTime(1900, 01, 01), user.LastLockedOutDate ?? new DateTime(1900, 01, 01));
    }

    #region Not Supported

    public override bool EnablePasswordRetrieval
    {
      get { return false; }
    }
    public override string GetPassword(string username, string answer)
    {
      throw new NotSupportedException("Consider using methods from WebSecurity module.");
    }

    public override bool EnablePasswordReset
    {
      get { return false; }
    }
    public override string ResetPassword(string username, string answer)
    {
      throw new NotSupportedException("Consider using methods from WebSecurity module.");
    }

    public override bool RequiresQuestionAndAnswer
    {
      get { return false; }
    }
    public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
    {
      throw new NotSupportedException("Consider using methods from WebSecurity module.");
    }

    public override void UpdateUser(MembershipUser user)
    {
      throw new NotSupportedException();
    }

    #endregion
  }
}
