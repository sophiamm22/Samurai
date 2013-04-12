using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;
using Samurai.Web.ViewModels.Tennis;

namespace Samurai.Services.Contracts.Async
{
  public interface IAsyncPredictionAdminService
  {
    int GetCountOfDaysPredictions(DateTime fixtureDate, string sport);
  }

  public interface IAsyncFootballPredictionAdminService : IAsyncPredictionAdminService
  {
    Task<IEnumerable<FootballPredictionViewModel>> GetFootballPredictions(IEnumerable<FootballFixtureViewModel> fixtures);
    Task<IEnumerable<FootballPredictionViewModel>> FetchFootballPredictions(IEnumerable<FootballFixtureViewModel> fixtures);
  }
  public interface IAsyncTennisPredictionAdminService : IAsyncPredictionAdminService
  {
    Task<IEnumerable<TennisFixtureViewModel>> GetTennisPredictions(DateTime matchDate);
    Task<IEnumerable<TennisFixtureViewModel>> FetchTennisPredictions(DateTime matchDate);
    Task<IEnumerable<TennisFixtureViewModel>> FetchTennisPredictionCoupons(DateTime matchDate);
    Task<TennisFixtureViewModel> GetSingleTennisPrediction(string playerASurname, string playerAFirstname, string playerBSurname, string playerBFirstname, int year, string tournamentSlug, bool updateStats = true);
  }
}
