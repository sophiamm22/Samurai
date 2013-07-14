using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Tennis;
using Samurai.Web.ViewModels.Value;

namespace Samurai.Services.Contracts
{
  public interface ITennisFacadeAdminService
  {
    IEnumerable<TennisFixtureViewModel> GetDaysSchedule(DateTime fixtureDate);
    IEnumerable<TennisMatchViewModel> FetchTennisResults(DateTime matchDate);
    IEnumerable<TennisFixtureViewModel> UpdateDaysSchedule(DateTime fixtureDate);
    IEnumerable<TournamentEventViewModel> GetTournamentEvents();
    void AddTournamentCouponURL(TournamentCouponURLViewModel viewModel);
    IEnumerable<ShowTournamentLadderChallengeViewModel> CalculateTournamentLadderChallenge(CalculateTournamentLadderChallengeViewModel viewModel);
    IEnumerable<TennisLadderViewModel> GetTournamentLadder(DateTime matchDate, string tournament);
    void AddAlias(string source, string playerName, string valueSamuraiName, string valueSamuraiFirstName);
  }
}
