using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Entities;

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

    public FootballFixtureViewModel GetFootballFixture(string dateString, string homeTeamSlug, string awayTeamSlug)
    {
      DateTime fixtureDate;
      if (!DateTime.TryParse(dateString, out fixtureDate))
        return null;

      var homeTeamEntity = this.fixtureRepository.GetTeamOrPlayer(homeTeamSlug);
      var awayTeamEntity = this.fixtureRepository.GetTeamOrPlayer(awayTeamSlug);

      var match = this.fixtureRepository.GetMatchFromTeamSelections(homeTeamEntity, awayTeamEntity, fixtureDate);

      return Mapper.Map<Match, FootballFixtureViewModel>(match);
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
