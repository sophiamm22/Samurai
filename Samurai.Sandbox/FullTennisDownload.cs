using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    //public void Get2013Calendar()
    //{
    //  var tennisService = this.container.Resolve<ITennisFacadeService>();
    //  var events = tennisService.GetTournamentEvents();
    //}

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
      while (true)
      {
        Console.WriteLine(string.Format("Enter URL for {0} via {1}", tournament, externalSource));
        var input = Console.ReadLine();
        if (Uri.IsWellFormedUriString(input, UriKind.RelativeOrAbsolute))
          return input;
        else
          Console.WriteLine("Not a recognised URL format, try again..");
      }
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
              throw new NotImplementedException();
            }
          }
        });
    }
  }
  
}
