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
  public interface IAsyncOddsService
  {
    OddsSourceViewModel FindOddsSource(string slug);
    SportViewModel FindSport(string slug);
    TournamentViewModel FindTournament(string slug);
    void AddTournamentCouponURL(TournamentCouponURLViewModel viewModel);
  }

  public interface IAsyncTennisOddsService : IAsyncOddsService
  {
    Task<TennisCouponViewModel> GetSingleTennisOdds(int matchID);
    Task<IEnumerable<TennisCouponViewModel>> GetAllTennisOdds(IEnumerable<int> matchIDs);
    Task<IEnumerable<TennisCouponViewModel>> GetAllTennisTodaysOdds(DateTime fixtureDate);

    Task<IEnumerable<TennisCouponViewModel>> FetchAllTennisOdds(DateTime date);
    Task<IEnumerable<TennisCouponViewModel>> FetchTennisOddsForTournamentSource(DateTime date, TournamentViewModel tournament, OddsSourceViewModel oddsSource);
    Task<IEnumerable<TennisCouponViewModel>> FetchCoupons(DateTime date, string tournament, string oddsSource);
  }

  public interface IAsyncFootballOddsService : IAsyncOddsService
  {
    Task<FootballCouponViewModel> GetSingleFootballOdds(int matchID);
    Task<IEnumerable<FootballCouponViewModel>> GetAllFootballOdds(IEnumerable<int> matchIDs);
    Task<IEnumerable<FootballCouponViewModel>> GetAllFootballTodaysOdds(DateTime fixtureDate);

    Task<IEnumerable<FootballCouponViewModel>> FetchAllFootballOdds(DateTime date);
    Task<IEnumerable<FootballCouponViewModel>> FetchFootballOddsForTournamentSource(DateTime date, TournamentViewModel tournament, OddsSourceViewModel oddsSource);
    Task<IEnumerable<FootballCouponViewModel>> FetchCoupons(DateTime date, string tournament, string oddsSource);
  }

}
