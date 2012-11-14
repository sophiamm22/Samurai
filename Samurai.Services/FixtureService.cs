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
using Samurai.Domain.Value;
using Model = Samurai.Domain.Model;

namespace Samurai.Services
{
  public class FixtureService : IFixtureService
  {
    private readonly IFixtureStrategyProvider fixtureProvider;
    private readonly IFixtureRepository fixtureRepository;

    public FixtureService(IFixtureRepository fixtureRepository,
      IFixtureStrategyProvider fixtureProvider)
    {
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (fixtureProvider == null) throw new ArgumentNullException("fixtureProvider");
      this.fixtureRepository = fixtureRepository;
      this.fixtureProvider = fixtureProvider;
    }

    public TournamentViewModel GetTournament(string slug)
    {
      var tournament = this.fixtureRepository.GetTournamentFromSlug(slug);
      if (tournament == null) return null;

      return Mapper.Map<Tournament, TournamentViewModel>(tournament);
    }

    public TeamPlayerViewModel GetTeamOrPlayer(string slug)
    {
      var teamEntity = this.fixtureRepository.GetTeamOrPlayer(slug);
      if (teamEntity == null) return null;
      return Mapper.Map<TeamPlayer, TeamPlayerViewModel>(teamEntity);
    }

    public FootballFixtureViewModel GetFootballFixture(DateTime fixtureDate, string homeTeam, string awayTeam)
    {
      
      var homeTeamEntity = this.fixtureRepository.GetTeamOrPlayerFromName(homeTeam);
      var awayTeamEntity = this.fixtureRepository.GetTeamOrPlayerFromName(awayTeam);

      var match = this.fixtureRepository.GetMatchFromTeamSelections(homeTeamEntity, awayTeamEntity, fixtureDate);

      return Mapper.Map<Match, FootballFixtureViewModel>(match);
    }

    public IEnumerable<FootballFixtureViewModel> FetchSkySportsFootballFixtures(DateTime fixtureDate)
    {
      var fixtureStrategy = this.fixtureProvider.CreateFixtureStrategy(Model.SportEnum.Football);
      var fixtures = fixtureStrategy.UpdateFixtures(fixtureDate);

      return Mapper.Map<IEnumerable<Match>, IEnumerable<FootballFixtureViewModel>>(fixtures);
    }

    public IEnumerable<FootballFixtureViewModel> FetchSkySportsFootballResults(DateTime fixtureDate)
    {
      var fixtureStrategy = this.fixtureProvider.CreateFixtureStrategy(Model.SportEnum.Football);
      var fixtures = fixtureStrategy.UpdateResults(fixtureDate);

      return Mapper.Map<IEnumerable<Match>, IEnumerable<FootballFixtureViewModel>>(fixtures);
    }


    public IEnumerable<FootballFixtureSummaryViewModel> GetFootballFixturesByDate(DateTime dateString, string league)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<FootballFixtureSummaryViewModel> GetFootballFixturesByGameweek(int gameWeek, string league)
    {
      throw new NotImplementedException();
    }
  }
}
