using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.Services
{
  public class FixtureService : IFixtureService
  {
    private readonly IFixtureRepository fixtureRepository;

    public FixtureService(IFixtureRepository fixtureRepository)
    {
      if (fixtureRepository == null) throw new NullReferenceException("fixtureRepository");
      this.fixtureRepository = fixtureRepository;
    }

    public FootballFixtureViewModel GetFootballFixture(string dateString, string homeTeam, string awayTeam)
    {
      var dateParts = dateString.Split('-');

      var fixtureDate = new DateTime(int.Parse(dateParts[2]), int.Parse(dateParts[1]), int.Parse(dateParts[0]));

      var homeTeamEntity = this.fixtureRepository.GetTeamFromSkySportsName(homeTeam);
      var awayTeamEntity = this.fixtureRepository.GetTeamFromSkySportsName(awayTeam);

      var match = this.fixtureRepository.GetFootballFixtureFromTeamSelections(homeTeamEntity, awayTeamEntity, fixtureDate);

      return new FootballFixtureViewModel();
    }
    public IEnumerable<FootballFixtureSummaryViewModel> GetFootballFixturesByDate(string league, string dateString)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<FootballFixtureSummaryViewModel> GetFootballFixturesByGameweek(string league, string gameWeek)
    {
      throw new NotImplementedException();
    }

    public bool LeagueExists(string league)
    {
      throw new NotImplementedException();
    }
  }
}
