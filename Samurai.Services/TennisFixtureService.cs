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
  public class TennisFixtureService : FixtureService, ITennisFixtureService
  {
    public TennisFixtureService(IFixtureRepository fixtureRepository,
      IFixtureStrategyProvider fixtureProvider, IStoredProceduresRepository storedProcRepository)
      : base(fixtureRepository, fixtureProvider, storedProcRepository)
    { }

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
      var fixtureStrategy = this.fixtureProvider.CreateFixtureStrategy(Model.SportEnum.Tennis);
      var fixtures = fixtureStrategy.UpdateResults(matchDate);

      return Mapper.Map<IEnumerable<Match>, IEnumerable<TennisMatchViewModel>>(fixtures);
    }
  
  }
}
