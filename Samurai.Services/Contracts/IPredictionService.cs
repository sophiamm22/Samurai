using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Web.ViewModels;

namespace Samurai.Services.Contracts
{
  public interface IPredictionService
  {
    IEnumerable<FootballFixtureViewModel> GetFootballPredictions(IEnumerable<FootballFixtureViewModel> fixtures);
    IEnumerable<FootballFixtureViewModel> FetchFootballPredictions(IEnumerable<FootballFixtureViewModel> fixtures);
  }
}
