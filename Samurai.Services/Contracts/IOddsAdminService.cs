using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;
using Samurai.Web.ViewModels.Tennis;
using Samurai.Web.ViewModels.Value;

namespace Samurai.Services.Contracts
{
  public interface IOddsAdminService
  {
    OddsSourceViewModel FindOddsSource(string slug);
    SportViewModel FindSport(string slug);
    TournamentViewModel FindTournament(string slug);
    void AddTournamentCouponURL(TournamentCouponURLViewModel viewModel);
  }

  public interface ITennisOddsAdminService : IOddsAdminService
  {
    TennisCouponViewModel GetSingleTennisOdds(DateTime date, TennisFixtureViewModel fixture);
    IEnumerable<TennisCouponViewModel> GetAllTennisOdds(DateTime date, IEnumerable<TennisFixtureViewModel> fixtures);
    IEnumerable<TennisCouponViewModel> FetchAllTennisOdds(DateTime date);
  }

  public interface IFootballOddsAdminService : IOddsAdminService
  {
    IEnumerable<FootballCouponViewModel> FetchAllFootballOdds(DateTime date);
    IEnumerable<FootballCouponViewModel> FetchAllPreScreenedFootballOdds(DateTime date);
    IEnumerable<FootballCouponViewModel> FetchCoupons(DateTime date, string tournament, string oddsSource, string sport, bool getOdds, bool prescreen);
  }
}
