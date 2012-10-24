using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;

using Samurai.Domain.Entities;
using Samurai.Web.ViewModels;

namespace Samurai.Services.AutoMapper
{
  public class FootballFixtureProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<Match, FootballFixtureViewModel>().IgnoreAllNonExisting();
      Mapper.CreateMap<Match, FootballFixtureViewModel>().ForMember(x => x.LeagueAndSeason, opt =>
        { opt.MapFrom(x => string.Format("{0} - {1}", x.TournamentEvent.Tournament.TournamentName, x.TournamentEvent.EventName)); });
      Mapper.CreateMap<Match, FootballFixtureViewModel>().ForMember(x => x.ScoreLine, opt =>
        opt.ResolveUsing<ScoreLineResolver>());
        
    }
  }

  public class ScoreLineResolver : ValueResolver<Match, string>
  {
    protected override string ResolveCore(Match source)
    {
      var observedOutcome = source.ObservedOutcomes.FirstOrDefault();
      if (observedOutcome == null)
        return "Not played";
      return observedOutcome.ScoreOutcome.ToString();
    }
  }
}
