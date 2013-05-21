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
    IEnumerable<OddViewModel> GetSingleTennisOdds(DateTime date, TennisFixtureViewModel fixture);
    IEnumerable<OddViewModel> GetAllTennisOdds(DateTime date, IEnumerable<TennisFixtureViewModel> fixtures);

    IEnumerable<OddViewModel> FetchAllTennisOdds(DateTime date);
    IEnumerable<OddViewModel> FetchTennisOddsForTournamentSource(DateTime date, TournamentViewModel tournament, OddsSourceViewModel oddsSource);
    IEnumerable<OddViewModel> FetchCoupons(DateTime date, string tournament, string oddsSource, string sport, bool getOdds, bool prescreen);
  }

  public interface IFootballOddsService : IOddsService
  {
    IEnumerable<OddViewModel> FetchAllFootballOdds(DateTime date);
    IEnumerable<OddViewModel> FetchFootballOddsForTournamentSource(DateTime date, TournamentViewModel tournament, OddsSourceViewModel oddsSource);
    IEnumerable<OddViewModel> FetchAllPreScreenedFootballOdds(DateTime date);
    IEnumerable<OddViewModel> FetchCoupons(DateTime date, string tournament, string oddsSource, string sport, bool getOdds, bool prescreen);
  }
}
