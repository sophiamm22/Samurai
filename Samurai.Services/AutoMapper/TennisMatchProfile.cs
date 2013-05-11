using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;

using Samurai.Domain.Entities;
using Samurai.Web.ViewModels;

namespace Samurai.Services.AutoMapper
{
  public class TennisMatchProfile : Profile
  {
    protected override void Configure()
    {
      //come back to, likely to be scrapped anyway
      Mapper.CreateMap<Match, TennisMatchViewModel>().IgnoreAllNonExisting();
      //Mapper.CreateMap<Match, TennisMatchViewModel>().ForMember(x => x.League, opt =>
      //{ opt.MapFrom(x => x.TournamentEvent.Tournament.TournamentName); });
      //Mapper.CreateMap<Match, TennisMatchViewModel>().ForMember(x => x.Season, opt =>
      //{ opt.MapFrom(x => x.TournamentEvent.EventName); });
      Mapper.CreateMap<Match, TennisMatchViewModel>().ForMember(x => x.ScoreLine, opt =>
        opt.ResolveUsing<ScoreLineResolver>());

    }
  }
}
