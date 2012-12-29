using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;

namespace Samurai.Services
{
  public class FootballServiceFacade : IFootballFacadeService
  {
    protected readonly IFootballFixtureService footballFixtureService;
    protected readonly IFootballPredictionService footballPredictionService;
    protected readonly IFootballOddsService footballOddsService;

    public FootballServiceFacade(IFootballFixtureService footballFixtureService,
      IFootballPredictionService footballPredictionService, IFootballOddsService footballOddsService)
    {
      if (footballFixtureService == null) throw new ArgumentNullException("footballFixtureService");
      if (footballPredictionService == null) throw new ArgumentNullException("footballPredictionService");
      if (footballOddsService == null) throw new ArgumentNullException("footballOddsService");

      this.footballFixtureService = footballFixtureService;
      this.footballPredictionService = footballPredictionService;
      this.footballOddsService = footballOddsService;
    }

    public IEnumerable<FootballFixtureViewModel> RetrieveDaysSchedule(DateTime fixtureDate)
    {
      var ret = new List<FootballFixtureViewModel>();

      var footballFixtures = RetrieveDaysFixtures(fixtureDate);
      var footballPredictions = RetrieveDaysPredictions(fixtureDate, footballFixtures);
      var footballOdds = RetrieveDaysOdds(fixtureDate);

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

    private IEnumerable<FootballFixtureViewModel> RetrieveDaysFixtures(DateTime fixtureDate)
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

    private Dictionary<string, FootballPredictionViewModel> RetrieveDaysPredictions(DateTime fixtureDate, IEnumerable<FootballFixtureViewModel> footballFixtures)
    {
      Dictionary<string, FootballPredictionViewModel> daysPredictions;
      var daysPredictionCount = this.footballPredictionService.GetCountOfDaysPredictions(fixtureDate, "Football");
      if (daysPredictionCount == 0)
        daysPredictions = this.footballPredictionService.FetchFootballPredictions(footballFixtures).ToDictionary(f => f.MatchIdentifier);
      else
        daysPredictions = this.footballPredictionService.GetFootballPredictions(footballFixtures).ToDictionary(f => f.MatchIdentifier);

      return daysPredictions;
    }

    private Dictionary<string, FootballCouponViewModel> RetrieveDaysOdds(DateTime fixtureDate)
    {
      var daysOdds = this.footballOddsService.FetchAllFootballOddsNew(fixtureDate).ToDictionary(o => o.MatchIdentifier);

      return daysOdds;
    }

  }
}
