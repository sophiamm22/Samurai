using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels.Tennis;
using Samurai.Web.ViewModels.Value;

namespace Samurai.Services.Contracts.Async
{
  public interface IAsyncTennisFacadeClientService
  {
    Task<IEnumerable<TennisFixtureViewModel>> GetDaysSchedule(DateTime fixtureDate);
    Task<IEnumerable<OddViewModel>> GetDaysOdds(DateTime fixtureDate);
    DateTime GetLatestDate();
  }
}
