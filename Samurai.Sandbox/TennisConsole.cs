using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Castle.Windsor;

using Samurai.Domain.Exceptions;
using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Tennis;
using Samurai.Web.ViewModels.Value;

namespace Samurai.Sandbox
{
  public class TennisConsole
  {
    private readonly ITennisFacadeService tennisService;

    public IEnumerable<TennisFixtureViewModel> Fixtures { get; set; }

    public TennisConsole(ITennisFacadeService tennisService)
    {
      if (tennisService == null) throw new ArgumentNullException("tennisService");

      this.tennisService = tennisService;
    }

    public void TennisMenu()
    {
      while (true)
      {
        Console.WriteLine("Value-Samurai -- Tennis Menu");
        Console.WriteLine("Select from the list below..");
        Console.WriteLine("----------------------------");
        Console.WriteLine("1.\tGet Calendar");
        Console.WriteLine("2.\tFetch Day's Schedule");
        Console.WriteLine("3.\tGet Day's Schedule");
        Console.WriteLine("");
        Console.WriteLine("4.\tReturn to main menu");
        Console.WriteLine("----------------------------");
        var numberString = Console.ReadLine();
        int number;
        if (!int.TryParse(numberString, out number))
        {
          Console.WriteLine("You fucking moron!");
        }
        else
        {
          if (number == 1)
            Get2013Calendar();
          else if (number == 2)
            FetchTennisSchedule();
          else if (number == 3)
            GetTennisSchedule();
          else
            break;
        }
      }
    }

    private void Get2013Calendar()
    {
      var events = tennisService.GetTournamentEvents();
    }

    private void FetchTennisSchedule()
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
          Fixtures = tennisService.UpdateDaysSchedule(date);
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
        AddTournamentCouponURLs(missingURLs);
        AddMissingAlias(missingAlias, date);
      }
    }

    private void GetTennisSchedule()
    {
      Console.WriteLine("Enter the date to get full tennis schedule (dd/mm/yy)");
      var dateString = Console.ReadLine();
      DateTime date;
      if (!DateTime.TryParse(dateString, out date))
      {
        Console.WriteLine("You fucking moron!");
        return;
      }
      try
      {
        Fixtures = tennisService.GetDaysSchedule(date);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private void AddTournamentCouponURLs(IEnumerable<MissingTournamentCouponURL> missingURLs)
    {
      missingURLs.ToList().ForEach(x =>
      {
        var url = GetTournamentCouponURL(x.ExternalSource, x.Tournament);
        var viewModel = new TournamentCouponURLViewModel
        {
          ExternalSource = x.ExternalSource,
          Tournament = x.Tournament,
          URL = url
        };
        this.tennisService.AddTournamentCouponURL(viewModel);
      });
    }

    private string GetTournamentCouponURL(string externalSource, string tournament)
    {
      int count = 1;
      while (count <= 3)
      {
        Console.WriteLine(string.Format("Enter URL for {0} via {1}", tournament, externalSource));
        var input = Console.ReadLine();
        if (Uri.IsWellFormedUriString(input, UriKind.RelativeOrAbsolute))
          return input;
        else
          Console.WriteLine("Not a recognised URL format, try again..");
        count++;
      }
      throw new ArgumentException("url");
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
          var tournamentLadder = this.tennisService.GetTournamentLadder(date, x.Tournament);
          foreach (var externalSourceGroup in x.ExternalSourceGroups)
          {
            foreach (var teamOrPlayer in externalSourceGroup.TeamsOrPlayers)
            {
              var playerFullName = GetMissingAlias(tournamentLadder, externalSourceGroup.ExternalSource, teamOrPlayer);
              var playerNames = playerFullName.Split(',').Select(y => y.Trim());

              this.tennisService.AddAlias(externalSourceGroup.ExternalSource, teamOrPlayer, 
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
