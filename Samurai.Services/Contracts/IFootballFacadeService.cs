using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels.Football;

namespace Samurai.Services.Contracts
{
  public interface IFootballFacadeService
  {
    IEnumerable<FootballFixtureViewModel> UpdateDaysSchedule(DateTime fixtureDate);
  }
}
