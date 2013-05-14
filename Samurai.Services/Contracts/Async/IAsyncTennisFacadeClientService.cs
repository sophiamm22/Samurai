using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels.Tennis;

namespace Samurai.Services.Contracts.Async
{
  public interface IAsyncTennisFacadeClientService
  {
    Task<IEnumerable<TennisFixtureViewModel>> GetDaysSchedule(DateTime fixtureDate);
    Task<IEnumerable<TennisCouponViewModel>> GetDaysOdds(DateTime fixtureDate);
    DateTime GetLatestDate();
  }
}
