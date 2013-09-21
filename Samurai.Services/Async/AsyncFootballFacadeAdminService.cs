using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Services.Contracts.Async;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Value;
using Samurai.Web.ViewModels.Football;
using Samurai.Domain.Infrastructure;
using Samurai.Domain.Model;

namespace Samurai.Services.Async
{
  public class AsyncFootballFacadeAdminService : IAsyncFootballFacadeAdminService
  {
    protected readonly IAsyncFootballFixtureService footballFixtureService;
    protected readonly IAsyncFootballPredictionService footballPredictionService;
    protected readonly IAsyncFootballOddsService footballOddsService;

    public AsyncFootballFacadeAdminService(IAsyncFootballFixtureService footballFixtureService,
      IAsyncFootballPredictionService footballPredictionService, IAsyncFootballOddsService footballOddsService)
    {
      if (footballFixtureService == null) throw new ArgumentNullException("footballFixtureService");
      if (footballPredictionService == null) throw new ArgumentNullException("footballPredictionService");
      if (footballOddsService == null) throw new ArgumentNullException("footballOddsService");

      this.footballFixtureService = footballFixtureService;
      this.footballPredictionService = footballPredictionService;
      this.footballOddsService = footballOddsService;
    }

    public async Task<IEnumerable<FootballFixtureViewModel>> UpdateDaysSchedule(DateTime fixtureDate)
    {
      ProgressReporterProvider.Current.ReportProgress(string.Format("Getting Days Football Schedule for {0}", fixtureDate.ToShortDateString()), ReporterImportance.High, ReporterAudience.Admin);

      var ret = new List<FootballFixtureViewModel>();

      var footballFixtures = await UpdateDaysFixtures(fixtureDate);
      var footballPredictions = await UpdateDaysPredictions(fixtureDate, footballFixtures);
      var footballOdds = await UpdateDaysOdds(fixtureDate);

      foreach (var footballFixture in footballFixtures)
      {
        FootballPredictionViewModel prediction;
        IEnumerable<OddViewModel> odds;

        prediction = footballPredictions.ContainsKey(footballFixture.MatchIdentifier) ? footballPredictions[footballFixture.MatchIdentifier] : null;
        odds = footballOdds.ContainsKey(footballFixture.Id) ? footballOdds[footballFixture.Id] : null;

        footballFixture.Predictions = prediction;
        footballFixture.Odds = odds;

        ret.Add(footballFixture);
      }

      return ret;
    }

    public async Task<IEnumerable<FootballFixtureViewModel>> UpdateDaysResults(DateTime fixtureDate)
    {
      ProgressReporterProvider.Current.ReportProgress(string.Format("Getting Days Football Results for {0}", fixtureDate.ToShortDateString()), ReporterImportance.High, ReporterAudience.Admin);

      return await 
        this.footballFixtureService
            .FetchSkySportsFootballResults(fixtureDate);
    }

    public IEnumerable<FootballLadderViewModel> GetTournamentLadder(DateTime matchDate, string tournament)
    {
      ProgressReporterProvider.Current.ReportProgress(string.Format("Getting Tournament Ladder for {0} on {1}", tournament, matchDate.ToShortDateString()), ReporterImportance.High, ReporterAudience.Admin);

      return this.footballFixtureService
                 .GetTournamentLadder(matchDate, tournament);
    }

    public void AddAlias(string source, string playerName, string valueSamuraiName)
    {
      ProgressReporterProvider.Current.ReportProgress(string.Format("Adding alias for {0} at {1}", playerName, source), ReporterImportance.High, ReporterAudience.Admin);

      this.footballFixtureService.AddAlias(source, playerName, valueSamuraiName);
    }

    private async Task<IEnumerable<FootballFixtureViewModel>> UpdateDaysFixtures(DateTime fixtureDate)
    {
      var footballFixtures = new List<FootballFixtureViewModel>();
      var daysMatchCount = this.footballFixtureService.GetCountOfDaysMatches(fixtureDate, "Football");
      if (daysMatchCount == 0)
        footballFixtures.AddRange(await this.footballFixtureService.FetchSkySportsFootballFixtures(fixtureDate));
      else
        footballFixtures.AddRange(this.footballFixtureService.GetFootballFixturesByDate(fixtureDate));

      if (footballFixtures.Count == 0)
        return Enumerable.Empty<FootballFixtureViewModel>();

      return footballFixtures;
    }

    private async Task<Dictionary<string, FootballPredictionViewModel>> UpdateDaysPredictions(DateTime fixtureDate, IEnumerable<FootballFixtureViewModel> footballFixtures)
    {
      Dictionary<string, FootballPredictionViewModel> daysPredictions;
      var daysPredictionCount = this.footballPredictionService.GetCountOfDaysPredictions(fixtureDate, "Football");
      if (daysPredictionCount == 0)
        daysPredictions = (await this.footballPredictionService.FetchFootballPredictions(footballFixtures)).ToDictionary(f => f.MatchIdentifier);
      else
      {
        daysPredictions = (await this.footballPredictionService.GetFootballPredictions(footballFixtures)).ToDictionary(f => f.MatchIdentifier, f => f);
      }

      return daysPredictions;
    }

    private async Task<Dictionary<int, List<OddViewModel>>> UpdateDaysOdds(DateTime fixtureDate)
    {
      var groupedOdds = new Dictionary<int, List<OddViewModel>>();

      var daysOdds = (await this.footballOddsService
            .FetchAllFootballOdds(fixtureDate)).ToList();

      foreach (var odd in daysOdds)
      {
        if (!groupedOdds.ContainsKey(odd.MatchId))
          groupedOdds.Add(odd.MatchId, new List<OddViewModel>());
        groupedOdds[odd.MatchId].Add(odd);
      }

      return groupedOdds;
    }

  }
}
