using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;

using Samurai.Domain.Entities;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Tennis;
using Samurai.Web.ViewModels.Value;
using Samurai.Domain.Model;

namespace Samurai.Services.AutoMapper
{
  public class TennisFixtureViewModelProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<TennisMatchDetail, TennisFixtureViewModel>().IgnoreAllNonExisting();
      Mapper.CreateMap<TennisMatchDetail, TennisFixtureViewModel>().ForMember(x => x.Tournament, opt =>
        opt.MapFrom(x => x.TournamentName));
      Mapper.CreateMap<TennisMatchDetail, TennisFixtureViewModel>().ForMember(x => x.Year, opt =>
        opt.MapFrom(x => x.MatchDate.AddDays(7).Year));
      Mapper.CreateMap<TennisMatchDetail, TennisFixtureViewModel>().ForMember(x => x.PlayerASurname, opt =>
        opt.MapFrom(x => x.TeamOrPlayerA));
      Mapper.CreateMap<TennisMatchDetail, TennisFixtureViewModel>().ForMember(x => x.PlayerBSurname, opt =>
        opt.MapFrom(x => x.TeamOrPlayerB));

      Mapper.CreateMap<GenericMatchDetail, TennisFixtureViewModel>().IgnoreAllNonExisting();
      Mapper.CreateMap<GenericMatchDetail, TennisFixtureViewModel>().ForMember(x => x.ScoreLine, opt =>
        { opt.MapFrom(x => x.ObservedOutcome); });

      Mapper.CreateMap<TennisFixtureViewModel, TennisFixtureViewModel>().IgnoreAllNonExisting();


    }
  }
}
