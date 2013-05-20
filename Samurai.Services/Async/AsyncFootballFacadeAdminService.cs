using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Services.Contracts.Async;
using Samurai.Web.ViewModels;
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
        FootballPredictionViewModel predictionDecider;
        FootballCouponOutcomeViewModel oddsDecider;

        predictionDecider = footballPredictions.ContainsKey(footballFixture.MatchIdentifier) ? footballPredictions[footballFixture.MatchIdentifier] : null;
        oddsDecider = footballOdds.ContainsKey(footballFixture.MatchIdentifier) ? footballOdds[footballFixture.MatchIdentifier] : null;

        ret.Add(FootballFixtureViewModel.CreateCombination(footballFixture, predictionDecider, oddsDecider));
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

    private async Task<Dictionary<string, FootballCouponOutcomeViewModel>> UpdateDaysOdds(DateTime fixtureDate)
    {
      var groupedCoupons = new Dictionary<string, List<FootballCouponOutcomeViewModel>>();

      var daysCoupons = (await this.footballOddsService
            .FetchAllFootballOdds(fixtureDate)).ToList();
      foreach (var coupon in daysCoupons)
      {
        if (!groupedCoupons.ContainsKey(coupon.MatchIdentifier))
          groupedCoupons.Add(coupon.MatchIdentifier, new List<FootballCouponOutcomeViewModel>());
        groupedCoupons[coupon.MatchIdentifier].Add(coupon);
      }

      var ret = Mapper.Map<Dictionary<string, List<FootballCouponOutcomeViewModel>>, Dictionary<string, FootballCouponOutcomeViewModel>>(groupedCoupons);

      return ret;
    }

  }
}
