using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using Samurai.SqlDataAccess;
using WebMatrix.WebData;
using System.Web.Security;

namespace Samurai.SqlDataAccess.Migrations
{
  internal sealed class Configuration : DbMigrationsConfiguration<ValueSamuraiContext>
  {
    public Configuration()
    {
      AutomaticMigrationsEnabled = true;
    }

    protected override void Seed(ValueSamuraiContext context)
    {
      WebSecurity.InitializeDatabaseConnection(
        "ValueSamuraiContext",
        "UserProfile",
        "UserProfileID_pk",
        "UserName",
        autoCreateTables: true);

      SeedUsersAndRoles();

    }

    private void SeedUsersAndRoles()
    {
      if (!Roles.RoleExists("Administrator"))
        Roles.CreateRole("Administrator");

      if (!WebSecurity.UserExists("martinstaniforth"))
        WebSecurity.CreateUserAndAccount(
          "martinstaniforth",
          "password"/*,
          new { EmailAddress = "martinstaniforth@gmail.com" }*/);

      if (!Roles.GetRolesForUser("martinstaniforth").Contains("Administrator"))
        Roles.AddUsersToRoles(new[] { "martinstaniforth" }, new[] { "Administrator" });
    }

    //private void Seed

  }
}
