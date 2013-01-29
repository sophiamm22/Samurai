using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;

using Samurai.Domain.Entities;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;
using Samurai.Web.ViewModels.Value;
using Samurai.Domain.Model;

namespace Samurai.Services.AutoMapper
{
  public class FootballLadderViewModelProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<TeamPlayer, FootballLadderViewModel>().IgnoreAllNonExisting();
      Mapper.CreateMap<TeamPlayer, FootballLadderViewModel>().ForMember(x => x.TeamName, 
        opt => opt.MapFrom(x => x.Name));
    }
  }
}
