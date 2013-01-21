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
  public class FullTennisDownload
  {
    private readonly ITennisFacadeService tennisService;
    private readonly DateTime date;

    public IEnumerable<TennisFixtureViewModel> Fixtures { get; set; }

    public FullTennisDownload(ITennisFacadeService tennisService, DateTime date)
    {
      if (tennisService == null) throw new ArgumentNullException("tennisService");
      if (date == null) throw new ArgumentNullException("date");

      this.tennisService = tennisService;
      this.date = date;
    }

    public void PopulateDatabaseNew()
    {
      while (true)
      {
        var missingURLs = new List<MissingTournamentCouponURL>();
        var missingAlias = new List<MissingAlias>();
        try
        {
          Fixtures = tennisService.UpdateDaysSchedule(this.date);
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
        AddMissingAlias(missingAlias);
      }
    }

    public void Get2013Calendar()
    {
      var events = tennisService.GetTournamentEvents();
    }

    //public void PopulateDatabase()
    //{
    //  GetPredictions();
    //  GetOdds();
    //}

    //private void GetPredictions()
    //{
    //  var predictionService = this.container.Resolve<ITennisPredictionService>();
    //  var predictions = predictionService.FetchTennisPredictions(this.date);
    //}

    //private void GetOdds()
    //{
    //  var oddsService = this.container.Resolve<ITennisOddsService>();
    //  var fullFixtureDetails = oddsService.FetchAllTennisOdds(this.date);
    //}

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

    private void AddMissingAlias(IEnumerable<MissingAlias> missingAlias)
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
          var tournamentLadder = this.tennisService.GetTournamentLadder(this.date, x.Tournament);
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
                      .ForEach(x => Console.WriteLine(string.Format("{0}\t{2},{3}", x.Position, x.PlayerSurname.ToUpper(), x.PlayerFirstName)));
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
