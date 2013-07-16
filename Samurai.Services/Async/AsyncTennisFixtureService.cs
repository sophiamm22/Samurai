using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Services.Contracts.Async;
using Samurai.Domain.Value.Async;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Web.ViewModels;
using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Web.ViewModels.Tennis;
using Samurai.Domain.APIModel;
using Model = Samurai.Domain.Model;

namespace Samurai.Services.Async
{
  public class AsyncTennisFixtureService : AsyncFixtureService, IAsyncTennisFixtureService
  {
    protected readonly IAsyncTennisFixtureStrategy fixtureStrategy;

    public AsyncTennisFixtureService(IFixtureRepository fixtureRepository,
      IAsyncTennisFixtureStrategy fixtureStrategy, IStoredProceduresRepository storedProcRepository)
      : base(fixtureRepository, storedProcRepository)
    {
      if (fixtureStrategy == null) throw new ArgumentNullException("fixtureStrategy");
      this.fixtureStrategy = fixtureStrategy;
    }

    public IEnumerable<TennisMatchViewModel> GetTennisMatches(DateTime matchDate)
    {
      var matches = this.fixtureRepository.GetDaysTennisMatches(matchDate);

      return Mapper.Map<IEnumerable<Match>, IEnumerable<TennisMatchViewModel>>(matches);
    }

    public TennisMatchViewModel GetTennisMatch(string playerAName, string playerBName, DateTime matchDate)
    {
      var match = this.fixtureRepository.GetTennisMatch(playerAName, playerBName, matchDate);
      if (match == null) return null;
      return Mapper.Map<Match, TennisMatchViewModel>(match);
    }

    public async Task<IEnumerable<TennisFixtureViewModel>> FetchTennisResults(DateTime matchDate)
    {
      var fixtures = await
        this.fixtureStrategy
            .UpdateResults(matchDate);

      var fixturesDTO = Mapper.Map<IEnumerable<GenericMatchDetailQuery>, IEnumerable<Model.GenericMatchDetail>>(fixtures);

      return Mapper.Map<IEnumerable<Model.GenericMatchDetail>, IEnumerable<TennisFixtureViewModel>>(fixturesDTO);
    }

    public async Task<IEnumerable<TournamentEventViewModel>> FetchTournamentEvents()
    {
      var tournamentEvents = await
        this.fixtureStrategy
            .UpdateTournamentEvents();

      return Mapper.Map<IEnumerable<TournamentEvent>, IEnumerable<TournamentEventViewModel>>(tournamentEvents);
    }

    public async Task<IEnumerable<TennisLadderViewModel>> FetchTournamentLadder(DateTime matchDate, string tournament)
    {
      var year = matchDate.AddDays(3).Year;
      var tournamentSlug
        = this.fixtureRepository
              .GetTournament(tournament)
              .Slug;

      var apiDetails = await
        this.fixtureStrategy
            .GetTournamentDetail(tournamentSlug, year);

      var apiLadder =
        apiDetails.TournamentLadders
                  .OrderBy(x => x.Position);

      return Mapper.Map<IEnumerable<APITournamentLadder>, IEnumerable<TennisLadderViewModel>>(apiLadder);
    }
  }
}
