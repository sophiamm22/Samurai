using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels.Football;

namespace Samurai.Services.Contracts.Async
{
  public interface IAsyncFootballFacadeClientService
  {
    Task<IEnumerable<FootballFixtureViewModel>> GetDaysSchedule(DateTime fixtureDate);
    DateTime GetLatestDate();
  }
}
