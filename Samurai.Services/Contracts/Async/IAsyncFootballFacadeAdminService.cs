using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels.Football;

namespace Samurai.Services.Contracts.Async
{
  public interface IAsyncFootballFacadeAdminService
  {
    void AddAlias(string source, string playerName, string valueSamuraiName);
    Task<IEnumerable<FootballFixtureViewModel>> UpdateDaysSchedule(DateTime fixtureDate);
    Task<IEnumerable<FootballFixtureViewModel>> UpdateDaysResults(DateTime fixtureDate);
    IEnumerable<FootballLadderViewModel> GetTournamentLadder(DateTime matchDate, string tournament);

  }
}
