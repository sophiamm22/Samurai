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
  public interface IOddsService
  {
    OddsSourceViewModel FindOddsSource(string slug);
    SportViewModel FindSport(string slug);
    TournamentViewModel FindTournament(string slug);
    void AddTournamentCouponURL(TournamentCouponURLViewModel viewModel);
  }

  public interface ITennisOddsService : IOddsService
  {
    TennisCouponOutcomeViewModel GetSingleTennisOdds(DateTime date, TennisFixtureViewModel fixture);
    IEnumerable<TennisCouponOutcomeViewModel> GetAllTennisOdds(DateTime date, IEnumerable<TennisFixtureViewModel> fixtures);

    IEnumerable<TennisCouponOutcomeViewModel> FetchAllTennisOdds(DateTime date);
    IEnumerable<TennisCouponOutcomeViewModel> FetchTennisOddsForTournamentSource(DateTime date, TournamentViewModel tournament, OddsSourceViewModel oddsSource);
    IEnumerable<TennisCouponOutcomeViewModel> FetchCoupons(DateTime date, string tournament, string oddsSource, string sport, bool getOdds, bool prescreen);
  }

  public interface IFootballOddsService : IOddsService
  {
    IEnumerable<FootballCouponOutcomeViewModel> FetchAllFootballOdds(DateTime date);
    IEnumerable<FootballCouponOutcomeViewModel> FetchFootballOddsForTournamentSource(DateTime date, TournamentViewModel tournament, OddsSourceViewModel oddsSource);
    IEnumerable<FootballCouponOutcomeViewModel> FetchAllPreScreenedFootballOdds(DateTime date);
    IEnumerable<FootballCouponOutcomeViewModel> FetchCoupons(DateTime date, string tournament, string oddsSource, string sport, bool getOdds, bool prescreen);
  }
}
