using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;
using Samurai.Web.ViewModels.Tennis;
using Samurai.Web.ViewModels.Value;

namespace Samurai.Services.Contracts.Async
{
  public interface IAsyncOddsAdminService
  {
    OddsSourceViewModel FindOddsSource(string slug);
    SportViewModel FindSport(string slug);
    TournamentViewModel FindTournament(string slug);
    void AddTournamentCouponURL(TournamentCouponURLViewModel viewModel);
  }

  public interface IAsyncTennisOddsAdminService : IAsyncOddsAdminService
  {
    Task<TennisCouponViewModel> GetSingleTennisOdds(DateTime date, TennisFixtureViewModel fixture);
    Task<IEnumerable<TennisCouponViewModel>> GetAllTennisOdds(DateTime date, IEnumerable<TennisFixtureViewModel> fixtures);

    Task<IEnumerable<TennisCouponViewModel>> FetchAllTennisOdds(DateTime date);
    Task<IEnumerable<TennisCouponViewModel>> FetchTennisOddsForTournamentSource(DateTime date, TournamentViewModel tournament, OddsSourceViewModel oddsSource);
    Task<IEnumerable<TennisCouponViewModel>> FetchCoupons(DateTime date, string tournament, string oddsSource);
  }

  public interface IAsyncFootballOddsAdminService : IAsyncOddsAdminService
  {
    Task<IEnumerable<FootballCouponViewModel>> FetchAllFootballOdds(DateTime date);
    Task<IEnumerable<FootballCouponViewModel>> FetchFootballOddsForTournamentSource(DateTime date, TournamentViewModel tournament, OddsSourceViewModel oddsSource);
    Task<IEnumerable<FootballCouponViewModel>> FetchCoupons(DateTime date, string tournament, string oddsSource);
  }

}
