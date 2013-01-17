using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Tennis;
using Samurai.Web.ViewModels.Value;

namespace Samurai.Services
{
  public class TennisFacadeService : ITennisFacadeService
  {
    protected readonly ITennisFixtureService tennisFixtureService;
    protected readonly ITennisPredictionService tennisPredictionService;
    protected readonly ITennisOddsService tennisOddsService;

    public TennisFacadeService(ITennisFixtureService tennisFixtureService,
      ITennisPredictionService tennisPredictionService, ITennisOddsService tennisOddsService)
    {
      if (tennisFixtureService == null) throw new ArgumentNullException("tennisFixtureService");
      if (tennisPredictionService == null) throw new ArgumentNullException("tennisPredictionService");
      if (tennisOddsService == null) throw new ArgumentNullException("tennisOddsService");

      this.tennisFixtureService = tennisFixtureService;
      this.tennisPredictionService = tennisPredictionService;
      this.tennisOddsService = tennisOddsService;
    }

    public IEnumerable<TennisFixtureViewModel> UpdateDaysSchedule(DateTime fixtureDate)
    {
      var ret = new List<TennisFixtureViewModel>();

      var tennisFixturesAndPredictions = UpdateDaysFixturesAndPredicitons(fixtureDate);
      var tennisOdds = UpdateDaysOdds(fixtureDate);

      foreach (var tennisFixture in tennisFixturesAndPredictions)
      {
        TennisCouponViewModel oddsDecider;

        oddsDecider = tennisOdds.ContainsKey(tennisFixture.MatchIdentifier) ? tennisOdds[tennisFixture.MatchIdentifier] : null;

        ret.Add(TennisFixtureViewModel.CreateCombination(tennisFixture, oddsDecider));
      }

      return ret;
    }

    public IEnumerable<TournamentEventViewModel> GetTournamentEvents()
    {
      return this.tennisFixtureService.GetTournamentEvents();
    }

    public void AddTournamentCouponURL(TournamentCouponURLViewModel viewModel)
    {
      this.tennisOddsService.AddTournamentCouponURL(viewModel);
    }

    private IEnumerable<TennisFixtureViewModel> UpdateDaysFixturesAndPredicitons(DateTime fixtureDate)
    {
      var tennisFixtures = new List<TennisFixtureViewModel>();
      var daysMatchCount = this.tennisPredictionService.GetCountOfDaysPredictions(fixtureDate, "Tennis");
      if (daysMatchCount == 0)
        tennisFixtures.AddRange(this.tennisPredictionService.FetchTennisPredictionsNew(fixtureDate));
      else
        tennisFixtures.AddRange(this.tennisPredictionService.GetTennisPredictions(fixtureDate));

      if (tennisFixtures.Count == 0)
        return Enumerable.Empty<TennisFixtureViewModel>();

      return tennisFixtures;
    }

    private Dictionary<string, TennisCouponViewModel> UpdateDaysOdds(DateTime matchDate)
    {
      var groupedCoupons = new Dictionary<string, List<TennisCouponViewModel>>();

      var daysCoupons =
        this.tennisOddsService
            .FetchAllTennisOddsNew(matchDate);

      foreach (var coupon in daysCoupons)
      {
        if (!groupedCoupons.ContainsKey(coupon.MatchIdentifier))
          groupedCoupons.Add(coupon.MatchIdentifier, new List<TennisCouponViewModel>());
        groupedCoupons[coupon.MatchIdentifier].Add(coupon);
      }

      var ret = Mapper.Map<Dictionary<string, List<TennisCouponViewModel>>, Dictionary<string, TennisCouponViewModel>>(groupedCoupons);

      return ret;
    }

    public IEnumerable<TennisLadderViewModel> GetTournamentLadder(DateTime matchDate, string tournament)
    {
      throw new NotImplementedException();
    }
  }
}
