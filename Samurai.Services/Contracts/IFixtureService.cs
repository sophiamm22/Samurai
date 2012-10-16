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
    FootballFixtureViewModel GetFootballFixture(string dateString, string homeTeam, string awayTeam);
    IEnumerable<FootballFixtureSummaryViewModel> GetFootballFixturesByDate(string league, string dateString);
    IEnumerable<FootballFixtureSummaryViewModel> GetFootballFixturesByGameweek(string league, string gameWeek);
    bool LeagueExists(string league);
  }
}
