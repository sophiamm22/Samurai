using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;

using Samurai.Core;
using Samurai.Domain.Entities;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Tennis;
using Samurai.Web.ViewModels.Value;
using Samurai.Domain.Model;
using Samurai.Domain.APIModel;

namespace Samurai.Services.AutoMapper
{
  public class TennisLadderViewModelProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<APITournamentLadder, TennisLadderViewModel>()
        .IgnoreAllNonExisting()
        .ForMember(x => x.PlayerFirstName, opt => { opt.MapFrom(x => x.PlayerName.Split(',')[1].Trim()); })
        .ForMember(x => x.PlayerSurname, opt => { opt.MapFrom(x => x.PlayerName.Split(',')[0].Trim()); })
        .ForMember(x => x.PlayerFirstNameSlug, opt => { opt.MapFrom(x => x.PlayerFirstName.ToHyphenated()); })
        .ForMember(x => x.PlayerSurnameSlug, opt => { opt.MapFrom(x => x.PlayerSurname.ToHyphenated()); });

    }
  }
}
