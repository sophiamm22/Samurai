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
using Samurai.Domain.Infrastructure;
using Samurai.Domain.Model;
using Samurai.Domain.Repository;

namespace Samurai.Sandbox
{
  public class TennisConsole
  {
    private readonly ITennisFacadeAdminService tennisService;
    private readonly ITwitterClient twitter;
    private readonly IWebRepositoryProvider webRepositoryProvider;

    public TennisConsole(ITennisFacadeAdminService tennisService, 
      IWebRepositoryProvider webRepositoryProvider = null, ITwitterClient twitter = null)
    {
      if (tennisService == null) throw new ArgumentNullException("tennisService");
      
      this.tennisService = tennisService;
      this.webRepositoryProvider = webRepositoryProvider;
      this.twitter = twitter;
    }

    public void TennisMenu()
    {
      while (true)
      {
        ProgressReporterProvider.Current.ReportProgress("Value-Samurai -- Main Menu", ReporterImportance.High, ReporterAudience.Admin);
        ProgressReporterProvider.Current.ReportProgress("1.\tGet Calendar", ReporterImportance.Medium, ReporterAudience.Admin);
        ProgressReporterProvider.Current.ReportProgress("2.\tFetch Day's Schedule", ReporterImportance.Medium, ReporterAudience.Admin);
        ProgressReporterProvider.Current.ReportProgress("3.\tFetch Day's Results", ReporterImportance.Medium, ReporterAudience.Admin);
        ProgressReporterProvider.Current.ReportProgress("4.\tCalculate Ladder Challenge", ReporterImportance.Medium, ReporterAudience.Admin);
        ProgressReporterProvider.Current.ReportProgress("", ReporterImportance.Medium, ReporterAudience.Admin);
        ProgressReporterProvider.Current.ReportProgress("5.\tReturn to main menu", ReporterImportance.Low, ReporterAudience.Admin);

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
            FetchTennisResults();
          else if (number == 4)
            GetTournamentLadderChallenge();
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
        ProgressReporterProvider.Current.ReportProgress("Enter the date to fetch full tennis schedule (dd/mm/yy)", ReporterImportance.High, ReporterAudience.Admin);

        var dateString = Console.ReadLine();
        DateTime date;
        if (!DateTime.TryParse(dateString, out date))
        {
          Console.WriteLine("You fucking moron!");
          break;
        }
        var missingURLs = new List<MissingTournamentCouponURLObject>();
        var missingAlias = new List<MissingTeamPlayerAliasObject>();
        try
        {
          var fixtures = tennisService.UpdateDaysSchedule(date);
          break;
        }
        catch (MissingTournamentCouponURLException tcmEx)
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

    private void FetchTennisResults()
    {
      ProgressReporterProvider.Current.ReportProgress("Enter the date to fetch tennis results (dd/mm/yy)", ReporterImportance.High, ReporterAudience.Admin);

      var dateString = Console.ReadLine();
      DateTime date;
      if (!DateTime.TryParse(dateString, out date))
      {
        Console.WriteLine("You fucking moron!");
        return;
      }
      try
      {
        var fixtures = tennisService.FetchTennisResults(date);

        foreach (var fixture in fixtures)
        {
          ProgressReporterProvider.Current.ReportProgress(
            string.Format("Picked up {0} vs. {1} on {2}", fixture.PlayerAFirstName, fixture.PlayerBFirstName, date.ToShortDateString()),
            ReporterImportance.Medium,
            ReporterAudience.Admin);
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private void GetTournamentLadderChallenge()
    {
      ProgressReporterProvider.Current.ReportProgress("Enter the tournament name", ReporterImportance.High, ReporterAudience.Admin);
      var tournamentString = Console.ReadLine();
      ProgressReporterProvider.Current.ReportProgress("Enter the tournament year", ReporterImportance.High, ReporterAudience.Admin);
      var tournamentYearString = Console.ReadLine();

      var viewModel = new CalculateTournamentLadderChallengeViewModel
      {
        Tournament = tournamentString,
        StartDate = new DateTime(int.Parse(tournamentYearString), 06, 29),
        AllowRecalculation = false
      };

      var challengeLadders = this.tennisService.CalculateTournamentLadderChallenge(viewModel);

      int round = 0;
      int playerCount = 0;
      int maxLengthWinner = 0;
      int maxLengthLoser = 0;
      foreach (var challengeLadder in challengeLadders)
      {
        if (round != challengeLadder.RoundNumber)
        {
          maxLengthWinner = 5 + challengeLadders.Max(x => x.ExpectedWinner.Length);
          maxLengthLoser = 5 + challengeLadders.Max(x => x.ExpectedLoser.Length);
          round++;
          if (!(round == 1 && playerCount == 0))
            Console.ReadLine();
          ProgressReporterProvider.Current.ReportProgress(string.Format("Round {0}", challengeLadder.RoundNumber), ReporterImportance.High, ReporterAudience.Admin);
       }
        else if (playerCount % 16 == 0)
        {
          ProgressReporterProvider.Current.ReportProgress(new String(' ', 10), ReporterImportance.High, ReporterAudience.Admin);
          Console.ReadLine();
        }
        ProgressReporterProvider.Current.ReportProgress(
          string.Format("{0}bt.\t{1}with probability {2}", challengeLadder.ExpectedWinner.PadRight(maxLengthWinner, ' '), challengeLadder.ExpectedLoser.PadRight(maxLengthLoser, ' '), challengeLadder.Probability.ToString("###.00%").PadLeft(10)), 
          ReporterImportance.Medium, ReporterAudience.Admin);
        playerCount++;
      }
      Console.ReadLine();
    }

    private void AddTournamentCouponURLs(IEnumerable<MissingTournamentCouponURLObject> missingURLs)
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

    private void AddMissingAlias(IEnumerable<MissingTeamPlayerAliasObject> missingAlias, DateTime date)
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

    private void TweetPredictions(IEnumerable<TennisFixtureViewModel> fixtures)
    {
      var qualifyingPredictions =
        fixtures.Where(x => x.Predictions.PlayerAGames >= 70 && x.Predictions.PlayerBGames >= 70)
                .ToList();
      if (qualifyingPredictions.Count == 0) return;

      var webRepository = 
        this.webRepositoryProvider
            .CreateWebRepository(fixtures.First().MatchDate.Date);

      var auth = this.twitter.Auth();
      foreach (var prediction in qualifyingPredictions)
      {
        var tinyURL = webRepository.GetHTMLRaw(new Uri[] { new Uri(string.Format("http://tinyurl.com/api-create.php?url={0}", prediction.PredictionURL)) }, s => Console.WriteLine(s)).First();
        


        //var tweet = string.Format("{0} to bt. {1}, {2:P1} @ {3} {4}"  prediction.PlayerASurname
      }
      

    }
  }
  
}
