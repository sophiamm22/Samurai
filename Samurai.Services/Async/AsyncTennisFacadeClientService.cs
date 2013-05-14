using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Core;
using Samurai.Services.Contracts.Async;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Tennis;
using Samurai.Web.ViewModels.Value;
using Samurai.Domain.Model;
using Samurai.Domain.Infrastructure;

namespace Samurai.Services.Async
{
  public class AsyncTennisFacadeClientService : IAsyncTennisFacadeClientService
  {
    protected readonly IAsyncTennisFixtureService tennisFixtureService;
    protected readonly IAsyncTennisPredictionService tennisPredictionService;
    protected readonly IAsyncTennisOddsService tennisOddsService;

    public AsyncTennisFacadeClientService(IAsyncTennisFixtureService tennisFixtureService,
      IAsyncTennisPredictionService tennisPredictionService, IAsyncTennisOddsService tennisOddsService)
    {
      if (tennisFixtureService == null) throw new ArgumentNullException("tennisFixtureService");
      if (tennisPredictionService == null) throw new ArgumentNullException("tennisPredictionService");
      if (tennisOddsService == null) throw new ArgumentNullException("tennisOddsService");

      this.tennisFixtureService = tennisFixtureService;
      this.tennisPredictionService = tennisPredictionService;
      this.tennisOddsService = tennisOddsService;
    }

    public async Task<IEnumerable<TennisFixtureViewModel>> GetDaysSchedule(DateTime fixtureDate)
    {
      var groupedCoupons = new Dictionary<string, List<TennisCouponViewModel>>();
      
      var tennisFixtures = new List<TennisFixtureViewModel>();
      tennisFixtures.AddRange(await this.tennisPredictionService.GetTennisPredictions(fixtureDate));

      return tennisFixtures;
    }

    public async Task<IEnumerable<TennisCouponViewModel>> GetDaysOdds(DateTime fixtureDate)
    {
      return await
        this.tennisOddsService
            .GetAllTennisTodaysOdds(fixtureDate);
    }

    public DateTime GetLatestDate()
    {
      return this.tennisFixtureService.GetLatestDate();
    }

  }
}
