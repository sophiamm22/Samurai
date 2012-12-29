using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Domain.Value;
using Model = Samurai.Domain.Model;

namespace Samurai.Services
{
  public abstract class FixtureService : IFixtureService
  {
    protected readonly IFixtureStrategyProvider fixtureProvider;
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly IStoredProceduresRepository storedProcRepository;

    public FixtureService(IFixtureRepository fixtureRepository,
      IFixtureStrategyProvider fixtureProvider, IStoredProceduresRepository storedProcRepository)
    {
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (fixtureProvider == null) throw new ArgumentNullException("fixtureProvider");
      if (storedProcRepository == null) throw new ArgumentNullException("storedProcRepository");
      this.fixtureRepository = fixtureRepository;
      this.fixtureProvider = fixtureProvider;
      this.storedProcRepository = storedProcRepository;
    }

    public int GetCountOfDaysMatches(DateTime fixtureDate, string sport)
    {
      var count = this.fixtureRepository.GetDaysMatches(fixtureDate, sport).Count();
      return count;
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
  }

  public class FootballFixtureService : FixtureService, IFootballFixtureService
  {
    public FootballFixtureService(IFixtureRepository fixtureRepository,
      IFixtureStrategyProvider fixtureProvider, IStoredProceduresRepository storedProcRepository)
      : base(fixtureRepository, fixtureProvider, storedProcRepository)
    { }

    public FootballFixtureViewModel GetFootballFixture(DateTime fixtureDate, string homeTeam, string awayTeam)
    {
      var homeTeamEntity = this.fixtureRepository.GetTeamOrPlayerFromName(homeTeam);
      var awayTeamEntity = this.fixtureRepository.GetTeamOrPlayerFromName(awayTeam);

      var match = this.fixtureRepository.GetMatchFromTeamSelections(homeTeamEntity, awayTeamEntity, fixtureDate);

      return Mapper.Map<Match, FootballFixtureViewModel>(match);
    }

    public IEnumerable<FootballFixtureViewModel> FetchSkySportsFootballFixturesNew(DateTime fixtureDate)
    {
      var fixtureStrategy = this.fixtureProvider.CreateFixtureStrategy(Model.SportEnum.Football);
      var fixtures = fixtureStrategy.UpdateFixturesNew(fixtureDate);

      var fixturesDTO = Mapper.Map<IEnumerable<GenericMatchDetailQuery>, IEnumerable<Model.GenericMatchDetail>>(fixtures);
      return Mapper.Map<IEnumerable<Model.GenericMatchDetail>, IEnumerable<FootballFixtureViewModel>>(fixturesDTO);
    }

    public IEnumerable<FootballFixtureViewModel> FetchSkySportsFootballFixtures(DateTime fixtureDate)
    {
      var fixtureStrategy = this.fixtureProvider.CreateFixtureStrategy(Model.SportEnum.Football);
      var fixtures = fixtureStrategy.UpdateFixtures(fixtureDate);

      return Mapper.Map<IEnumerable<Match>, IEnumerable<FootballFixtureViewModel>>(fixtures);
    }

    public IEnumerable<FootballFixtureViewModel> FetchSkySportsFootballResultsNew(DateTime fixtureDate)
    {
      var fixtureStrategy = this.fixtureProvider.CreateFixtureStrategy(Model.SportEnum.Football);
      var fixtures = fixtureStrategy.UpdateResultsNew(fixtureDate);

      var fixturesDTO = Mapper.Map<IEnumerable<GenericMatchDetailQuery>, IEnumerable<Model.GenericMatchDetail>>(fixtures);
      return Mapper.Map<IEnumerable<Model.GenericMatchDetail>, IEnumerable<FootballFixtureViewModel>>(fixturesDTO);
    }

    public IEnumerable<FootballFixtureViewModel> FetchSkySportsFootballResults(DateTime fixtureDate)
    {
      var fixtureStrategy = this.fixtureProvider.CreateFixtureStrategy(Model.SportEnum.Football);
      var fixtures = fixtureStrategy.UpdateResults(fixtureDate);

      return Mapper.Map<IEnumerable<Match>, IEnumerable<FootballFixtureViewModel>>(fixtures);
    }

    public IEnumerable<FootballFixtureViewModel> GetFootballFixturesByDateNew(DateTime fixtureDate)
    {
      var fixtures = this.storedProcRepository
                         .GetGenericMatchDetails(fixtureDate, "Football")
                         .ToList();
      if (fixtures.Count == 0)
        return Enumerable.Empty<FootballFixtureViewModel>();
      else
      {
        var fixturesDTO = Mapper.Map<IEnumerable<GenericMatchDetailQuery>, IEnumerable<Model.GenericMatchDetail>>(fixtures);
        return Mapper.Map<IEnumerable<Model.GenericMatchDetail>, IEnumerable<FootballFixtureViewModel>>(fixturesDTO);
      }
    }

    public IEnumerable<FootballFixtureViewModel> GetFootballFixturesByDate(DateTime fixtureDate)
    {
      var fixtures = this.fixtureRepository.GetDaysMatchesWithTeamsTournaments(fixtureDate, "Football")
        .ToList();
      if (fixtures.Count == 0)
        return Enumerable.Empty<FootballFixtureViewModel>();
      else
        return Mapper.Map<IEnumerable<Match>, IEnumerable<FootballFixtureViewModel>>(fixtures);
    }

    public IEnumerable<FootballFixtureViewModel> GetFootballFixturesByDateLeague(DateTime fixtureDate, string league)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<FootballFixtureViewModel> GetFootballFixturesByGameweek(int gameWeek, string league)
    {
      throw new NotImplementedException();
    }
  }
}
