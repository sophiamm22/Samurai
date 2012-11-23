using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels;

namespace Samurai.Services.Contracts
{
  public interface IOddsService
  {
    OddsSourceViewModel FindOddsSource(string slug);
    SportViewModel FindSport(string slug);
    TournamentViewModel FindTournament(string slug);
  }

  public interface ITennisOddsService : IOddsService
  {
    IEnumerable<TennisMatchViewModel> FetchAllTennisOdds(DateTime date);
  }

  public interface IFootballOddsService : IOddsService
  {
    IEnumerable<FootballFixtureViewModel> FetchAllFootballOdds(DateTime date);
    IEnumerable<FootballFixtureViewModel> FetchAllPreScreenedFootballOdds(DateTime date);
    IEnumerable<FootballFixtureViewModel> FetchCoupons(DateTime date, string tournament, string oddsSource, string sport, bool getOdds, bool prescreen);
  }
}
