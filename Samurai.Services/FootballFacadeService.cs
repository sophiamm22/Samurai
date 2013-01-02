using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;

namespace Samurai.Services
{
  public class FootballFacadeService : IFootballFacadeService
  {
    protected readonly IFootballFixtureService footballFixtureService;
    protected readonly IFootballPredictionService footballPredictionService;
    protected readonly IFootballOddsService footballOddsService;

    public FootballFacadeService(IFootballFixtureService footballFixtureService,
      IFootballPredictionService footballPredictionService, IFootballOddsService footballOddsService)
    {
      if (footballFixtureService == null) throw new ArgumentNullException("footballFixtureService");
      if (footballPredictionService == null) throw new ArgumentNullException("footballPredictionService");
      if (footballOddsService == null) throw new ArgumentNullException("footballOddsService");

      this.footballFixtureService = footballFixtureService;
      this.footballPredictionService = footballPredictionService;
      this.footballOddsService = footballOddsService;
    }

    public IEnumerable<FootballFixtureViewModel> UpdateDaysSchedule(DateTime fixtureDate)
    {
      var ret = new List<FootballFixtureViewModel>();

      var footballFixtures = UpdateDaysFixtures(fixtureDate);
      var footballPredictions = UpdateDaysPredictions(fixtureDate, footballFixtures);
      var footballOdds = UpdateDaysOdds(fixtureDate);

      foreach (var footballFixture in footballFixtures)
      {
        FootballPredictionViewModel predictionDecider;
        FootballCouponViewModel oddsDecider;

        predictionDecider = footballPredictions.ContainsKey(footballFixture.MatchIdentifier) ? footballPredictions[footballFixture.MatchIdentifier] : null;
        oddsDecider = footballOdds.ContainsKey(footballFixture.MatchIdentifier) ? footballOdds[footballFixture.MatchIdentifier] : null;

        ret.Add(FootballFixtureViewModel.CreateCombination(footballFixture, predictionDecider, oddsDecider));
      }

      return ret;
    }

    private IEnumerable<FootballFixtureViewModel> UpdateDaysFixtures(DateTime fixtureDate)
    {
      var footballFixtures = new List<FootballFixtureViewModel>();
      var daysMatchCount = this.footballFixtureService.GetCountOfDaysMatches(fixtureDate, "Football");
      if (daysMatchCount == 0)
        footballFixtures.AddRange(this.footballFixtureService.FetchSkySportsFootballFixturesNew(fixtureDate));
      else
        footballFixtures.AddRange(this.footballFixtureService.GetFootballFixturesByDateNew(fixtureDate));

      if (footballFixtures.Count == 0)
        return Enumerable.Empty<FootballFixtureViewModel>();

      return footballFixtures;
    }

    private Dictionary<string, FootballPredictionViewModel> UpdateDaysPredictions(DateTime fixtureDate, IEnumerable<FootballFixtureViewModel> footballFixtures)
    {
      Dictionary<string, FootballPredictionViewModel> daysPredictions;
      var daysPredictionCount = this.footballPredictionService.GetCountOfDaysPredictions(fixtureDate, "Football");
      if (daysPredictionCount == 0)
        daysPredictions = this.footballPredictionService.FetchFootballPredictions(footballFixtures).ToDictionary(f => f.MatchIdentifier);
      else
      {
        daysPredictions = this.footballPredictionService.GetFootballPredictions(footballFixtures).ToDictionary(f => f.MatchIdentifier, f => f);
      }

      return daysPredictions;
    }

    private Dictionary<string, FootballCouponViewModel> UpdateDaysOdds(DateTime fixtureDate)
    {
      var groupedCoupons = new Dictionary<string, List<FootballCouponViewModel>>();

      var daysCoupons = this.footballOddsService.FetchAllFootballOddsNew(fixtureDate).ToList();
      foreach (var coupon in daysCoupons)
      {
        if (!groupedCoupons.ContainsKey(coupon.MatchIdentifier))
          groupedCoupons.Add(coupon.MatchIdentifier, new List<FootballCouponViewModel>());
        groupedCoupons[coupon.MatchIdentifier].Add(coupon);
      }

      var ret = Mapper.Map<Dictionary<string, List<FootballCouponViewModel>>, Dictionary<string, FootballCouponViewModel>>(groupedCoupons);

      return ret;
    }

  }
}
