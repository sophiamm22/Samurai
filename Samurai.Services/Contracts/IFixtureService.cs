using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;

namespace Samurai.Services.Contracts
{
  public interface IFixtureService
  {
    int GetCountOfDaysMatches(DateTime fixtureDate, string sport);
    TournamentViewModel GetTournament(string slug);
    TeamPlayerViewModel GetTeamOrPlayer(string slug);

  }

  public interface IFootballFixtureService : IFixtureService
  {
    FootballFixtureViewModel GetFootballFixture(DateTime fixtureDate, string homeTeam, string awayTeam);
    IEnumerable<FootballFixtureViewModel> FetchSkySportsFootballFixturesNew(DateTime fixtureDate);
    IEnumerable<FootballFixtureViewModel> FetchSkySportsFootballFixtures(DateTime fixtureDate);
    IEnumerable<FootballFixtureViewModel> FetchSkySportsFootballResultsNew(DateTime fixtureDate);
    IEnumerable<FootballFixtureViewModel> FetchSkySportsFootballResults(DateTime fixtureDate);
    IEnumerable<FootballFixtureViewModel> GetFootballFixturesByDateNew(DateTime fixtureDate);
    IEnumerable<FootballFixtureViewModel> GetFootballFixturesByDate(DateTime fixtureDate);
    IEnumerable<FootballFixtureViewModel> GetFootballFixturesByDateLeague(DateTime fixtureDate, string league);
    IEnumerable<FootballFixtureViewModel> GetFootballFixturesByGameweek(int gameWeek, string league);
  }

  public interface ITennisFixtureService : IFixtureService
  {
    IEnumerable<TennisMatchViewModel> GetTennisMatches(DateTime matchDate);
    TennisMatchViewModel GetTennisMatch(string playerAName, string playerBName, DateTime matchDate);
    IEnumerable<TennisMatchViewModel> FetchTennisResults(DateTime matchDate);
  }

}
