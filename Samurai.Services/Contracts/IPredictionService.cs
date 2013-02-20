using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;
using Samurai.Web.ViewModels.Tennis;

namespace Samurai.Services.Contracts
{
  public interface IPredictionService
  {
    int GetCountOfDaysPredictions(DateTime fixtureDate, string sport);
  }

  public interface ITennisPredictionService : IPredictionService
  {
    IEnumerable<TennisFixtureViewModel> GetTennisPredictions(DateTime matchDate);
    IEnumerable<TennisFixtureViewModel> FetchTennisPredictions(DateTime matchDate);
    IEnumerable<TennisFixtureViewModel> FetchTennisPredictionCoupons(DateTime matchDate);
  }
  public interface IFootballPredictionService : IPredictionService
  {
    IEnumerable<FootballPredictionViewModel> GetFootballPredictions(IEnumerable<FootballFixtureViewModel> fixtures);
    IEnumerable<FootballPredictionViewModel> FetchFootballPredictions(IEnumerable<FootballFixtureViewModel> fixtures);
  }
}
