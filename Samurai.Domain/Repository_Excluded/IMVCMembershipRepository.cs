using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Entities;

namespace Samurai.Domain.Repository
{
  public interface IMVCMembershipRepository
  {
    User AddUser(User user);
    User GetUserByEmail(string email);
    IEnumerable<User> GetUsersByEmail(string partialEmailToMatch, int pageIndex, int pageSize, out int totalRecords);
    IEnumerable<User> GetUsersByUserName(string partialNameToMatch, int pageIndex, int pageSize, out int totalRecords);
    IEnumerable<User> GetAllUsers(int pageIndex, int pageSize, out int totalRecords);
    User GetUserByUserName(string userName);
    User GetUserByOpenId(Guid openID);
    User GetUserByUserId(Guid userID);
    string GetUserNameByEmail(string email);
    string CreatePasswordHash(string password, string salt);
    string CreateSalt();
    User UserTouched(User user);
    void DeleteUser(User user);
    int GetNumberOfUsersOnline(DateTime since);

    Role GetRole(string role);
    void AddRole(Role role);
    IEnumerable<User> GetUsersInRole(string roleName, string usernameToMatch);
    IEnumerable<Role> GetAllRoles();
    void RemoveUser(User user);
    void RemoveRole(Role role);

    void SaveChanges();
  }
}
