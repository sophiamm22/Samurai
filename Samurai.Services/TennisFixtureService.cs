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
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Domain.Value;
using Samurai.Domain.APIModel;
using Model = Samurai.Domain.Model;
using Samurai.Web.ViewModels.Tennis;

namespace Samurai.Services
{
  public class TennisFixtureService : FixtureService, ITennisFixtureService
  {
    protected readonly ITennisFixtureStrategy fixtureStrategy;

    public TennisFixtureService(IFixtureRepository fixtureRepository,
      ITennisFixtureStrategy fixtureStrategy, IStoredProceduresRepository storedProcRepository)
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

    public IEnumerable<TennisMatchViewModel> FetchTennisResults(DateTime matchDate)
    {
      var fixtures = this.fixtureStrategy.UpdateResults(matchDate);

      return Mapper.Map<IEnumerable<GenericMatchDetailQuery>, IEnumerable<TennisMatchViewModel>>(fixtures);
    }

    public IEnumerable<TournamentEventViewModel> GetTournamentEvents()
    {
      var tournamentEvents = this.fixtureStrategy.UpdateTournamentEvents();

      return Mapper.Map<IEnumerable<TournamentEvent>, IEnumerable<TournamentEventViewModel>>(tournamentEvents);
    }

    public IEnumerable<TennisLadderViewModel> GetTournamentLadder(DateTime matchDate, string tournament)
    {
      var year = matchDate.AddDays(3).Year;
      var tournamentSlug 
        = this.fixtureRepository
              .GetTournament(tournament)
              .Slug;

      var apiDetails = this.fixtureStrategy.GetTournamentDetail(tournamentSlug, year);

      var apiLadder = 
        apiDetails.TournamentLadders
                  .OrderBy(x => x.Position);

      return Mapper.Map<IEnumerable<APITournamentLadder>, IEnumerable<TennisLadderViewModel>>(apiLadder);

    }

  }
}
