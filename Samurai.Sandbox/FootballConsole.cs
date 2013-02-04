using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Castle.Windsor;

using Samurai.Domain.Exceptions;
using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;
using Samurai.Domain.Entities;

namespace Samurai.Sandbox
{
  public class FootballConsole
  {
    private readonly IFootballFacadeService footballService;

    public IEnumerable<FootballFixtureViewModel> Fixtures { get; set; }

    public FootballConsole(IFootballFacadeService footballService)
    {
      if (footballService == null) throw new ArgumentNullException("footballService");

      this.footballService = footballService;
    }

    public void FootballMenu()
    {
      while (true)
      {
        Console.WriteLine("Value-Samurai -- Football Menu");
        Console.WriteLine("Select from the list below..");
        Console.WriteLine("------------------------------");
        Console.WriteLine("1.\tFetch Day's Schedule");
        Console.WriteLine("");
        Console.WriteLine("2.\tReturn to main menu");
        Console.WriteLine("------------------------------");
        var numberString = Console.ReadLine();
        int number;
        if (!int.TryParse(numberString, out number))
        {
          Console.WriteLine("You fucking moron!");
        }
        else
        {
          if (number == 1)
          {
            FetchFootballSchedule();
            break;
          }
          else
            break;
        }
      }
    }


    private void FetchFootballSchedule()
    {
      while (true)
      {
        Console.WriteLine("Enter the date to fetch full football schedule (dd/mm/yy)");
        var dateString = Console.ReadLine();
        DateTime date;
        if (!DateTime.TryParse(dateString, out date))
        {
          Console.WriteLine("You fucking moron!");
          break;
        }
        
        var missingURLs = new List<MissingTournamentCouponURL>();
        var missingTeamPlayerAlias = new List<MissingTeamPlayerAlias>();
        var missingBookmakerAlias = new List<MissingBookmakerAlias>();
        try
        {
          Fixtures = this.footballService.UpdateDaysSchedule(date);
          break;
        }
        catch (TournamentCouponURLMissingException tcmEx)
        {
          missingURLs.AddRange(tcmEx.MissingData);
        }
        catch (MissingTeamPlayerAliasException mtpaEx)
        {
          missingTeamPlayerAlias.AddRange(mtpaEx.MissingAlias);
        }
        catch (MissingBookmakerAliasException mbaEx)
        {
          missingBookmakerAlias.AddRange(mbaEx.MissingAlias);
          throw new NotImplementedException();
        }

        AddMissingAlias(missingTeamPlayerAlias, date);
      }
    }


    private void AddMissingAlias(IEnumerable<MissingTeamPlayerAlias> missingAlias, DateTime date)
    {
      var groupedAlias =
        (from alias in missingAlias
         group alias by alias.Tournament into tournamentGroups
         select new
         {
           Tournament = tournamentGroups.Key,
           ExternalSourceGroups =
             from tournamentGroup in tournamentGroups
             group tournamentGroup by tournamentGroup.ExternalSource into sourceGroup
             select new
             {
               ExternalSource = sourceGroup.Key,
               TeamsOrPlayers = sourceGroup.Select(x => x.TeamOrPlayerName)
             }
         })
        .ToList();
      groupedAlias.ForEach(x =>
      {
        var tournamentLadder = this.footballService.GetTournamentLadder(date, x.Tournament);
        foreach (var externalSourceGroup in x.ExternalSourceGroups)
        {
          foreach (var teamOrPlayer in externalSourceGroup.TeamsOrPlayers)
          {
            var team = GetMissingAlias(tournamentLadder, externalSourceGroup.ExternalSource, teamOrPlayer);

            this.footballService.AddAlias(externalSourceGroup.ExternalSource, teamOrPlayer, team);
          }
        }
      });
    }
    private string GetMissingAlias(IEnumerable<FootballLadderViewModel> tournamentLadder, string source, string playerName)
    {
      Console.WriteLine(string.Format("Select a team from the list of {0}", tournamentLadder.Count()));
      var count = 1;
      tournamentLadder.ToList()
                      .ForEach(x => 
                      {
                        Console.WriteLine(string.Format("{0}\t{1}", count, x.TeamName));
                        count++;
                      });
      Console.WriteLine(string.Format("..or enter the team's local name for {0} via {1}", playerName, source));
      var response = Console.ReadLine();
      if (Regex.IsMatch(response, @"\d+"))
      {
        var player = tournamentLadder.ElementAt(int.Parse(response) - 1);
        return player.TeamName;
      }
      else
        return response;
    }
  }
}
