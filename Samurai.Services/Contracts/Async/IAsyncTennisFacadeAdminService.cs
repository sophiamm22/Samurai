using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Tennis;
using Samurai.Web.ViewModels.Value;

namespace Samurai.Services.Contracts.Async
{
  public interface IAsyncTennisFacadeAdminService
  {
    Task<IEnumerable<TennisFixtureViewModel>> UpdateDaysSchedule(DateTime fixtureDate);
    Task<IEnumerable<TournamentEventViewModel>> FetchTournamentEvents();
    void AddTournamentCouponURL(TournamentCouponURLViewModel viewModel);
    Task<IEnumerable<TennisLadderViewModel>> FetchTournamentLadder(DateTime matchDate, string tournament);
    void AddAlias(string source, string playerName, string valueSamuraiName, string valueSamuraiFirstName);
  }
}
