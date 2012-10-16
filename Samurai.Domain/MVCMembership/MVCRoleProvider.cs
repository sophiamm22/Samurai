using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Collections.Specialized;

using Samurai.Domain.Repository;
using Samurai.Domain.Entities;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.Domain.MVCMembership
{
  //deprecated in favour of simple membership, dependencies were really hard to provide so I went for a simple membership provider
  public class MVCRoleProvider : RoleProvider
  {
    private IMVCMembershipRepository mvcMembershipService;

    public override string ApplicationName
    {
      get
      { return this.GetType().Assembly.GetName().Name.ToString(); }
      set
      { this.ApplicationName = this.GetType().Assembly.GetName().Name.ToString(); }
    }

    public override void Initialize(string name, NameValueCollection config)
    {
      if (config == null)
        throw new ArgumentException("config");
      if (string.IsNullOrEmpty(name))
        name = "MVCRoleProvider";
      if (string.IsNullOrEmpty(config["description"]))
      {
        config.Remove("description");
        config.Add("description", "Authentication Role Provider");
      }
      base.Initialize(name, config);

      //Dependency Injection in .NET, Mark Seemann, page 228
      this.mvcMembershipService = null; //see above, dependencies were a ball-ache so went for simple membership
      //var container = (IAccountContainer)HttpContext.Current.Application["container"];
      //this.mvcMembershipService = container.ResolveMembershipService();
    }

    public override void AddUsersToRoles(string[] usernames, string[] roleNames)
    {
      var users = new List<User>();
      var roles = new List<Role>();
      foreach (var username in usernames)
      {
        var user = this.mvcMembershipService.GetUserByUserName(username);
        if (user != null) users.Add(user);
      }
      foreach (var rolename in roleNames)
      {
        var role = this.mvcMembershipService.GetRole(rolename);
        if (role != null) roles.Add(role);
      }
      foreach (var user in users)
      {
        foreach (var role in roles)
        {
          if (user.Roles.Contains(role))
          {
            user.Roles.Add(role);
          }
        }
      }
      this.mvcMembershipService.SaveChanges();
    }
    public override void CreateRole(string roleName)
    {
      if (string.IsNullOrEmpty(roleName))
        throw new ArgumentException("roleName");

      var role = mvcMembershipService.GetRole(roleName);
      if (role != null)
        throw new InvalidOperationException("Role exists: " + roleName);

      var newRole = new Role()
      {
        RoleID_pk = Guid.NewGuid(),
        RoleName = roleName
      };
      mvcMembershipService.AddRole(newRole);
      mvcMembershipService.SaveChanges();
    }

    public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
    {
      if (string.IsNullOrEmpty(roleName))
        throw new ArgumentException("roleName");

      var role = this.mvcMembershipService.GetRole(roleName);
      if (role == null) throw new InvalidOperationException("Role not found");

      if (throwOnPopulatedRole)
      {
        if (role.Users.Any()) throw new InvalidOperationException("Role populated: " + roleName);
      }
      else
      {
        foreach (var user in role.Users)
        {
          var usr = user;
          this.mvcMembershipService.DeleteUser(usr);
        }
      }
      this.mvcMembershipService.RemoveRole(role);
      this.mvcMembershipService.SaveChanges();
      return true;
    }

    public override string[] FindUsersInRole(string roleName, string usernameToMatch)
    {
      if (string.IsNullOrEmpty(usernameToMatch))
        throw new ArgumentException("usernameToMatch");
      if (string.IsNullOrEmpty(roleName))
        throw new ArgumentException("roleName");

      return this.mvcMembershipService.GetUsersInRole(roleName, usernameToMatch).Select(u => u.Username).ToArray();

    }

    public override string[] GetAllRoles()
    {
      return this.mvcMembershipService.GetAllRoles().Select(r => r.RoleName).ToArray();
    }

    public override string[] GetRolesForUser(string username)
    {
      return this.mvcMembershipService.GetUserByUserName(username).Roles.Select(r => r.RoleName).ToArray();
    }

    public override string[] GetUsersInRole(string roleName)
    {
      if (string.IsNullOrEmpty(roleName)) throw new ArgumentException("roleName");

      var role = this.mvcMembershipService.GetRole(roleName);

      if (role == null) throw new InvalidOperationException("Role not found");
      return role.Users.Select(u => u.Username).ToArray();
    }

    public override bool IsUserInRole(string username, string roleName)
    {
      if (string.IsNullOrEmpty(username))
        throw new ArgumentException("username");
      if (string.IsNullOrEmpty(roleName))
        throw new ArgumentException("roleName");
      var user = this.mvcMembershipService.GetUserByUserName(username);
      var role = this.mvcMembershipService.GetRole(roleName);

      if (user == null || role == null) return false;

      return user.Roles.Contains(role);

    }

    public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
    {
      foreach (string username_loopVariable in usernames)
      {
        var username = username_loopVariable;

        var user = this.mvcMembershipService.GetUserByUserName(username);
        if (user != null)
        {
          foreach (string rolename_loopVariable in roleNames)
          {
            var rolename = rolename_loopVariable;
            var rl = rolename;
            var role = user.Roles.FirstOrDefault(r => r.RoleName == rl);
            if (role != null)
            {
              user.Roles.Remove(role);
            }
          }
        }
      }
      this.mvcMembershipService.SaveChanges();
    }

    public override bool RoleExists(string roleName)
    {
      if (string.IsNullOrEmpty(roleName))
        throw new ArgumentException("roleName");

      var role = this.mvcMembershipService.GetRole(roleName);
      return role != null;
    }

  }
}
