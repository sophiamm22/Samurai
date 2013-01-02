using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Tennis;

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

      //do stuff

      return ret;
    }

    private IEnumerable<TennisFixtureViewModel> UpdateDaysFixturesAndPredicitons(DateTime fixtureDate)
    {
      var tennisFixtures = new List<TennisFixtureViewModel>();
      var daysMatchCount = this.tennisPredictionService.GetCountOfDaysPredictions(fixtureDate, "Tennis");
      if (daysMatchCount == 0)
        tennisFixtures.AddRange(this.tennisPredictionService.FetchTennisPredictions(fixtureDate));
      else
        tennisFixtures.AddRange(this.tennisPredictionService.GetTennisPredictions(fixtureDate));

      if (tennisFixtures.Count == 0)
        return Enumerable.Empty<TennisFixtureViewModel>();

      return tennisFixtures;

    }
    
  }
}
