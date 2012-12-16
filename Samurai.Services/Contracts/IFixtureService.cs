using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels;

namespace Samurai.Services.Contracts
{
  public interface IFixtureService
  {
    TournamentViewModel GetTournament(string slug);
    TeamPlayerViewModel GetTeamOrPlayer(string slug);
  }

  public interface IFootballFixtureService : IFixtureService
  {
    FootballFixtureViewModel GetFootballFixture(DateTime fixtureDate, string homeTeam, string awayTeam);
    IEnumerable<FootballFixtureViewModel> FetchSkySportsFootballFixtures(DateTime fixtureDate);
    IEnumerable<FootballFixtureViewModel> FetchSkySportsFootballResults(DateTime fixtureDate);
    IEnumerable<FootballFixtureSummaryViewModel> GetFootballFixturesByDate(DateTime dateString, string league);
    IEnumerable<FootballFixtureSummaryViewModel> GetFootballFixturesByGameweek(int gameWeek, string league);
  }

  public interface ITennisFixtureService : IFixtureService
  {
    IEnumerable<TennisMatchViewModel> GetTennisMatches(DateTime matchDate);
    TennisMatchViewModel GetTennisMatch(string playerAName, string playerBName, DateTime matchDate);
    IEnumerable<TennisMatchViewModel> FetchTennisResults(DateTime matchDate);
  }

}
