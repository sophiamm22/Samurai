using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;

using Samurai.Domain.Entities;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;
using Samurai.Domain.Model;

namespace Samurai.Services.AutoMapper
{
  public class FootballFixtureViewModelProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<GenericMatchDetail, FootballFixtureViewModel>().IgnoreAllNonExisting();
      Mapper.CreateMap<GenericMatchDetail, FootballFixtureViewModel>().ForMember(x => x.League, opt =>
        { opt.MapFrom(x => x.TournamentEventName); });
      Mapper.CreateMap<GenericMatchDetail, FootballFixtureViewModel>().ForMember(x => x.HomeTeam, opt =>
        { opt.MapFrom(x => x.TeamOrPlayerA); });
      Mapper.CreateMap<GenericMatchDetail, FootballFixtureViewModel>().ForMember(x => x.AwayTeam, opt =>
        { opt.MapFrom(x => x.TeamOrPlayerB); });
      Mapper.CreateMap<GenericMatchDetail, FootballFixtureViewModel>().ForMember(x => x.ScoreLine, opt =>
        { opt.MapFrom(x => x.ObservedOutcome); });

      Mapper.CreateMap<FootballFixtureViewModel, FootballFixtureViewModel>().IgnoreAllNonExisting();
    }
  }
}
