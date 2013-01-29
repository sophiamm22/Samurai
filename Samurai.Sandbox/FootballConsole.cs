using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Castle.Windsor;

using Samurai.Domain.Exceptions;
using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;

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
        Console.WriteLine("Enter the date to fetch full tennis schedule (dd/mm/yy)");
        var dateString = Console.ReadLine();
        DateTime date;
        if (!DateTime.TryParse(dateString, out date))
        {
          Console.WriteLine("You fucking moron!");
          break;
        }
        
        var missingURLs = new List<MissingTournamentCouponURL>();
        var missingAlias = new List<MissingAlias>();
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
          missingAlias.AddRange(mtpaEx.MissingAlias);
        }
      }
    }

    private void AddMissingAlias(IEnumerable<MissingAlias> missingAlias, DateTime date)
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
            var playerFullName = GetMissingAlias(tournamentLadder, externalSourceGroup.ExternalSource, teamOrPlayer);
            var playerNames = playerFullName.Split(',').Select(y => y.Trim());

            this.footballService.AddAlias(externalSourceGroup.ExternalSource, teamOrPlayer,
              playerNames.ElementAt(0), playerNames.ElementAt(1));
          }
        }
      });
    }
    private string GetMissingAlias(IEnumerable<TennisLadderViewModel> tournamentLadder, string source, string playerName)
    {
      Console.WriteLine(string.Format("Select a player from the list by ladder position (1-{0})", tournamentLadder.Count()));
      tournamentLadder.ToList()
                      .ForEach(x => Console.WriteLine(string.Format("{0}\t{1},{2}", x.Position, x.PlayerSurname.ToUpper(), x.PlayerFirstName)));
      Console.WriteLine(string.Format("..or enter the player's local name in for the form 'Surname, FirstName' for {0} via {1}", playerName, source));
      var response = Console.ReadLine();
      if (Regex.IsMatch(response, @"\d+"))
      {
        var player = tournamentLadder.First(x => x.Position == int.Parse(response));
        return string.Format("{0}, {1}", player.PlayerSurname, player.PlayerFirstName);
      }
      else
        return response;
    }
  }
}
