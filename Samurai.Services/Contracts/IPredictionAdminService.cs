using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;
using Samurai.Web.ViewModels.Tennis;

namespace Samurai.Services.Contracts
{
  public interface IPredictionAdminService
  {
    int GetCountOfDaysPredictions(DateTime fixtureDate, string sport);
  }

  public interface IFootballPredictionAdminService : IPredictionAdminService
  {
    IEnumerable<FootballPredictionViewModel> GetFootballPredictions(IEnumerable<FootballFixtureViewModel> fixtures);
    IEnumerable<FootballPredictionViewModel> FetchFootballPredictions(IEnumerable<FootballFixtureViewModel> fixtures);
  }  
  public interface ITennisPredictionAdminService : IPredictionAdminService
  {
    IEnumerable<TennisFixtureViewModel> GetTennisPredictions(DateTime matchDate);
    IEnumerable<TennisFixtureViewModel> FetchTennisPredictions(DateTime matchDate);
    IEnumerable<TennisFixtureViewModel> FetchTennisPredictionCoupons(DateTime matchDate);
    TennisFixtureViewModel GetSingleTennisPrediction(string playerASurname, string playerAFirstname, string playerBSurname, string playerBFirstname, int year, string tournamentSlug, bool updateStats = true);
  }

}
