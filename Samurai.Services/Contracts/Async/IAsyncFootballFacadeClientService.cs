using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels.Football;
using Samurai.Web.ViewModels.Value;

namespace Samurai.Services.Contracts.Async
{
  public interface IAsyncFootballFacadeClientService
  {
    Task<IEnumerable<FootballFixtureViewModel>> GetDaysSchedule(DateTime fixtureDate);
    Task<IEnumerable<OddViewModel>> GetDaysOdds(DateTime fixtureDate);
    DateTime GetLatestDate();
  }
}
