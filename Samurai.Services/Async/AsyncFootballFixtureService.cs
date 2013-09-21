using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Services.Contracts;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Services.Contracts.Async;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;
using Samurai.Domain.Value.Async;
using Samurai.Domain.Exceptions;
using Model = Samurai.Domain.Model;

namespace Samurai.Services.Async
{
  public abstract class AsyncFixtureService : IAsyncFixtureService
  {
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly ISqlLinqStoredProceduresRepository linqStoredProcRepository;
    protected readonly ISqlStoredProceduresRepository sqlStoredProcRepository;

    public AsyncFixtureService(IFixtureRepository fixtureRepository,
      ISqlLinqStoredProceduresRepository linqStoredProcRepository, ISqlStoredProceduresRepository sqlStoredProcRepository)
    {
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (linqStoredProcRepository == null) throw new ArgumentNullException("storedProcRepository");
      if (sqlStoredProcRepository == null) throw new ArgumentNullException("sqlStoredProcRepository");
      this.fixtureRepository = fixtureRepository;
      this.linqStoredProcRepository = linqStoredProcRepository;
      this.sqlStoredProcRepository = sqlStoredProcRepository;
    }

    public int GetCountOfDaysMatches(DateTime fixtureDate, string sport)
    {
      var count = this.fixtureRepository.GetDaysMatches(fixtureDate, sport).Count();
      return count;
    }

    public TournamentViewModel GetTournamentFromSlug(string slug)
    {
      var tournament = this.fixtureRepository.GetTournamentFromSlug(slug);
      if (tournament == null) return null;

      return Mapper.Map<Tournament, TournamentViewModel>(tournament);
    }

    public TournamentViewModel GetTournament(string tournamentName)
    {
      var tournament = this.fixtureRepository.GetTournament(tournamentName);
      if (tournament == null) return null;

      return Mapper.Map<Tournament, TournamentViewModel>(tournament);
    }

    public TeamPlayerViewModel GetTeamOrPlayer(string slug)
    {
      var teamEntity = this.fixtureRepository.GetTeamOrPlayer(slug);
      if (teamEntity == null) return null;
      return Mapper.Map<TeamPlayer, TeamPlayerViewModel>(teamEntity);
    }

    public void AddAlias(string source, string playerName, string valueSamuraiName, string valueSamuraiFirstName = null)
    {
      var player
        = this.fixtureRepository
              .GetTeamOrPlayerFromNameAndMaybeFirstName(valueSamuraiName, valueSamuraiFirstName);
      var externalSource
        = this.fixtureRepository
              .GetExternalSource(source);

      this.fixtureRepository
          .CreateTeamPlayerExternalAlias(player, externalSource, playerName);
    }

    public DateTime GetLatestDate()
    {
      return
        this.fixtureRepository
            .GetLatestDate();
    }

    public void RecordMissingTeamPlayerAlias(IEnumerable<MissingTeamPlayerAliasObject> players)
    {
      var missingPlayers = new List<MissingTeamPlayerExternalSourceAlias>();
      foreach (var player in players)
      {
        missingPlayers.Add(new MissingTeamPlayerExternalSourceAlias()
        {
          ExternalSourceID = player.ExternalSourceID,
          TournamentID = player.TournamentID,
          TeamPlayer = player.TeamOrPlayerName
        });
      }

      this.fixtureRepository
          .AddMissingTeamPlayerAlias(missingPlayers);
    }
  }


  public class AsyncFootballFixtureService : AsyncFixtureService, IAsyncFootballFixtureService
  {
    protected readonly IAsyncFootballFixtureStrategy fixtureStrategy;

    public AsyncFootballFixtureService(IFixtureRepository fixtureRepository,
      IAsyncFootballFixtureStrategy fixtureStrategy, ISqlLinqStoredProceduresRepository linqStoredProcRepository,
      ISqlStoredProceduresRepository sqlStoredProcRepository)
      : base(fixtureRepository, linqStoredProcRepository, sqlStoredProcRepository)
    {
      if (fixtureStrategy == null) throw new ArgumentNullException("fixtureStrategy");
      this.fixtureStrategy = fixtureStrategy;
    }

    public FootballFixtureViewModel GetFootballFixture(DateTime fixtureDate, string homeTeam, string awayTeam)
    {
      var homeTeamEntity = 
        this.fixtureRepository
            .GetTeamOrPlayerFromName(homeTeam);
      var awayTeamEntity = 
        this.fixtureRepository
            .GetTeamOrPlayerFromName(awayTeam);

      var match = 
        this.fixtureRepository
            .GetMatchFromTeamSelections(homeTeamEntity, awayTeamEntity, fixtureDate);

      return Mapper.Map<Match, FootballFixtureViewModel>(match);
    }

    public async Task<IEnumerable<FootballFixtureViewModel>> FetchSkySportsFootballFixtures(DateTime fixtureDate)
    {
      var fixtures = await
        this.fixtureStrategy
            .UpdateFixtures(fixtureDate);

      var fixturesDTO = Mapper.Map<IEnumerable<GenericMatchDetailQuery>, IEnumerable<Model.GenericMatchDetail>>(fixtures);
      return Mapper.Map<IEnumerable<Model.GenericMatchDetail>, IEnumerable<FootballFixtureViewModel>>(fixturesDTO);
    }

    public async Task<IEnumerable<FootballFixtureViewModel>> FetchSkySportsFootballResults(DateTime fixtureDate)
    {
      var fixtures = await
        this.fixtureStrategy
            .UpdateResults(fixtureDate);

      var fixturesDTO = Mapper.Map<IEnumerable<GenericMatchDetailQuery>, IEnumerable<Model.GenericMatchDetail>>(fixtures);
      return Mapper.Map<IEnumerable<Model.GenericMatchDetail>, IEnumerable<FootballFixtureViewModel>>(fixturesDTO);
    }

    public IEnumerable<FootballFixtureViewModel> GetFootballPredictions(DateTime fixtureDate)
    {
      var fixtures = this.sqlStoredProcRepository
                         .GetDaysFootballPredictions(fixtureDate)
                         .ToList();
      if (fixtures.Count == 0)
        return Enumerable.Empty<FootballFixtureViewModel>();
      else
        return Mapper.Map<IEnumerable<DaysFootballPredictions>, IEnumerable<FootballFixtureViewModel>>(fixtures);
      
    }

    public IEnumerable<FootballFixtureViewModel> GetFootballFixturesByDate(DateTime fixtureDate)
    {
      var fixtures = this.linqStoredProcRepository
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

    public IEnumerable<FootballFixtureViewModel> GetFootballFixturesByDateLeague(DateTime fixtureDate, string league)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<FootballFixtureViewModel> GetFootballFixturesByGameweek(int gameWeek, string league)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<FootballLadderViewModel> GetTournamentLadder(DateTime matchDate, string tournament)
    {
      var teams
        = this.fixtureRepository
              .GetLeagueLadder(tournament, matchDate)
              .OrderBy(x => x.Name)
              .ToList();


      return Mapper.Map<IEnumerable<TeamPlayer>, IEnumerable<FootballLadderViewModel>>(teams);
    }
  }
}
