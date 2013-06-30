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
    Task<IEnumerable<OddViewModel>> GetPeriodTennisOdds(DateTime startDate, DateTime endDate);

    Task<IEnumerable<OddViewModel>> GetSingleTennisOdds(int matchID);
    Task<IEnumerable<OddViewModel>> GetAllTennisOdds(IEnumerable<int> matchIDs);
    Task<IEnumerable<OddViewModel>> GetAllTennisTodaysOdds(DateTime fixtureDate);

    Task<IEnumerable<OddViewModel>> FetchAllTennisOdds(DateTime date);
    Task<IEnumerable<OddViewModel>> FetchTennisOddsForTournamentSource(DateTime date, TournamentViewModel tournament, OddsSourceViewModel oddsSource);
    Task<IEnumerable<OddViewModel>> FetchCoupons(DateTime date, string tournament, string oddsSource);
  }

  public interface IAsyncFootballOddsService : IAsyncOddsService
  {
    Task<IEnumerable<OddViewModel>> GetSingleFootballOdds(int matchID);
    Task<IEnumerable<OddViewModel>> GetAllFootballOdds(IEnumerable<int> matchIDs);
    Task<IEnumerable<OddViewModel>> GetAllFootballTodaysOdds(DateTime fixtureDate);

    Task<IEnumerable<OddViewModel>> FetchAllFootballOdds(DateTime date);
    Task<IEnumerable<OddViewModel>> FetchFootballOddsForTournamentSource(DateTime date, TournamentViewModel tournament, OddsSourceViewModel oddsSource);
    Task<IEnumerable<OddViewModel>> FetchCoupons(DateTime date, string tournament, string oddsSource);
  }

}
