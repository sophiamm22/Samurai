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
using Model = Samurai.Domain.Model;

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
      var fixtures = this.fixtureStrategy.UpdateResultsNew(matchDate);

      return Mapper.Map<IEnumerable<GenericMatchDetailQuery>, IEnumerable<TennisMatchViewModel>>(fixtures);
    }

    public IEnumerable<TournamentEventViewModel> GetTournamentEvents()
    {
      var tournamentEvents = this.fixtureStrategy.UpdateTournamentEvents();

      return Mapper.Map<IEnumerable<TournamentEvent>, IEnumerable<TournamentEventViewModel>>(tournamentEvents);
    }

    public IEnumerable<Web.ViewModels.Tennis.TennisLadderViewModel> GetTournamentLadder(DateTime matchDate, string tournament)
    {
      throw new NotImplementedException();
    }
  }
}
