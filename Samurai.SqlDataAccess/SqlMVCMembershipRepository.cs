using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Web.Security;

using Infrastructure.Data;
using Samurai.Domain.Entities;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.SqlDataAccess
{
  public class SqlMVCMembershipRepository : GenericRepository, IMVCMembershipRepository
  {
    public SqlMVCMembershipRepository(DbContext context)
      : base(context)
    { }

    public User AddUser(User user)
    {
      Add<User>(user);
      Save<User>(user);
      return user;
    }

    public User GetUserByEmail(string email)
    {
      return First<User>(u => u.Email == email);
    }

    public IEnumerable<User> GetUsersByEmail(string partialEmailToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      var users = Get<User, string>(u => u.Email.Contains(partialEmailToMatch), u => u.Email, pageIndex, pageSize, SortOrder.Descending).ToList();
      totalRecords = users.Count();
      return users;
    }

    public IEnumerable<User> GetUsersByUserName(string partialNameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      var users = Get<User, string>(u => u.Username.Contains(partialNameToMatch), u => u.Username, pageIndex, pageSize, SortOrder.Descending).ToList();
      totalRecords = users.Count();
      return users;
    }

    public IEnumerable<User> GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
    {
      var users = Get<User, string>(u => u.Username, pageIndex, pageSize).ToList();
      totalRecords = users.Count();
      return users;
    }

    public User GetUserByUserName(string userName)
    {
      return FindOne<User>(u => u.Username == userName);
    }

    public User GetUserByOpenId(Guid openID)
    {
      return FindOne<User>(u => u.OpenID == openID);
    }

    public User GetUserByUserId(Guid userID)
    {
      return FindOne<User>(u => u.UserID_pk == userID);
    }

    public string GetUserNameByEmail(string email)
    {
      var user = FindOne<User>(u => u.Email == email);
      if (user == null) return string.Empty;
      return user.Username;
    }

    public string CreatePasswordHash(string password, string salt)
    {
      var saltAndPwd = string.Concat(password, salt);
      var hashedPwd = HashString(saltAndPwd, "sha1");
      return hashedPwd;
    }

    public string CreateSalt()
    {
      var rng = new RNGCryptoServiceProvider();
      byte[] buff = new byte[32];
      rng.GetBytes(buff);
      return Convert.ToBase64String(buff);
    }

    public User UserTouched(User user)
    {
      user.LastActivityDate = DateTime.UtcNow;
      SaveChanges();
      return user;
    }

    public void DeleteUser(User user)
    {
      throw new NotImplementedException();
    }

    public int GetNumberOfUsersOnline(DateTime since)
    {
      var usersOnline = GetQuery<User>(u => u.LastActivityDate > since);
      if (usersOnline == null) return 0;
      return usersOnline.Count();
    }

    public Role GetRole(string role)
    {
      return FindOne<Role>(r => r.RoleName == role);
    }

    public void AddRole(Role role)
    {
      Add<Role>(role);
      SaveChanges();
    }

    public IEnumerable<User> GetUsersInRole(string roleName, string usernameToMatch)
    {
      var role = GetRole(roleName);
      return role.Users.Where(u => u.Username.Contains(usernameToMatch));
    }

    public IEnumerable<Role> GetAllRoles()
    {
      return GetAll<Role>().ToList();
    }

    public void RemoveUser(User user)
    {
      Delete<User>(user);
    }

    public void RemoveRole(Role role)
    {
      Delete<Role>(role);
    }

    public void SaveChanges()
    {
      UnitOfWork.SaveChanges();
    }

    public static string HashString(string inputString, string hashName)
    {
      HashAlgorithm algorithm = HashAlgorithm.Create(hashName);
      if (algorithm == null)
      {
        throw new ArgumentException("Unrecognized hash name", "hashName");
      }
      byte[] hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
      return Convert.ToBase64String(hash);
    }
  }
}
