using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Web.ViewModels;

namespace Samurai.Services.Contracts
{
  public interface ITennisPredictionService
  {
    IEnumerable<TennisMatchViewModel> GetTennisPredictions(DateTime matchDate);
    IEnumerable<TennisMatchViewModel> FetchTennisPredictions(DateTime matchDate);
  }
  public interface IFootballPredictionService
  {
    IEnumerable<FootballFixtureViewModel> GetFootballPredictions(IEnumerable<FootballFixtureViewModel> fixtures);
    IEnumerable<FootballFixtureViewModel> FetchFootballPredictions(IEnumerable<FootballFixtureViewModel> fixtures);
  }
}
