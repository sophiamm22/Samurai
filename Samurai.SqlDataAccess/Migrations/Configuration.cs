using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Collections.Generic;
using WebMatrix.WebData;
using System.Web.Security;
using System.Diagnostics;
using System.Text;

using Samurai.Domain.Entities;
using Samurai.SqlDataAccess;

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
      SeedSamurai(context);

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

    private void SeedSamurai(ValueSamuraiContext context)
    {
      var seed = new SeedData();

      context.Set<Bookmaker>().AddOrUpdate(seed.Bookmakers);
      context.Set<Sport>().AddOrUpdate(seed.Sports);
      context.Set<Competition>().AddOrUpdate(seed.Competitions);
      context.Set<Tournament>().AddOrUpdate(seed.Tournaments);
      context.Set<TournamentEvent>().AddOrUpdate(seed.TournamentEvents);
      context.Set<Fund>().AddOrUpdate(seed.Funds);
      context.Set<ExternalSource>().AddOrUpdate(seed.ExternalSources);
      context.Set<MatchOutcome>().AddOrUpdate(seed.MatchOutcomes);
      context.Set<ScoreOutcome>().AddOrUpdate(seed.ScoreOutcomes);
      context.Set<TeamPlayer>().AddOrUpdate(seed.TeamsPlayers);
      context.Set<TeamPlayerExternalSourceAlias>().AddOrUpdate(seed.TeamPlayerExternalSourceAliass);

      SaveChanges(context);
    }

    private void SaveChanges(ValueSamuraiContext context)
    {
      try
      {
        context.SaveChanges();
      }
      catch (DbEntityValidationException ex)
      {
        StringBuilder sb = new StringBuilder();

        foreach (var failure in ex.EntityValidationErrors)
        {
          sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());

          foreach (var error in failure.ValidationErrors)
          {
            sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
            sb.AppendLine();
          }
        }

        throw new DbEntityValidationException("Entity Validation Failed - errors follow:\n" + sb.ToString());
      }
    }

  }
}
