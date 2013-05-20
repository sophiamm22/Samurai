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
    Task<IEnumerable<OddViewModel>> GetSingleTennisOdds(int matchID);
    Task<IEnumerable<OddViewModel>> GetAllTennisOdds(IEnumerable<int> matchIDs);
    Task<IEnumerable<OddViewModel>> GetAllTennisTodaysOdds(DateTime fixtureDate);

    Task<IEnumerable<TennisCouponOutcomeViewModel>> FetchAllTennisOdds(DateTime date);
    Task<IEnumerable<TennisCouponOutcomeViewModel>> FetchTennisOddsForTournamentSource(DateTime date, TournamentViewModel tournament, OddsSourceViewModel oddsSource);
    Task<IEnumerable<TennisCouponOutcomeViewModel>> FetchCoupons(DateTime date, string tournament, string oddsSource);
  }

  public interface IAsyncFootballOddsService : IAsyncOddsService
  {
    Task<FootballCouponOutcomeViewModel> GetSingleFootballOdds(int matchID);
    Task<IEnumerable<FootballCouponOutcomeViewModel>> GetAllFootballOdds(IEnumerable<int> matchIDs);
    Task<IEnumerable<FootballCouponOutcomeViewModel>> GetAllFootballTodaysOdds(DateTime fixtureDate);

    Task<IEnumerable<FootballCouponOutcomeViewModel>> FetchAllFootballOdds(DateTime date);
    Task<IEnumerable<FootballCouponOutcomeViewModel>> FetchFootballOddsForTournamentSource(DateTime date, TournamentViewModel tournament, OddsSourceViewModel oddsSource);
    Task<IEnumerable<FootballCouponOutcomeViewModel>> FetchCoupons(DateTime date, string tournament, string oddsSource);
  }

}
