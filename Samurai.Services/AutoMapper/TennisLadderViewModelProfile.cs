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
using Samurai.Domain.APIModel;

namespace Samurai.Services.AutoMapper
{
  public class TennisLadderViewModelProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<APITournamentLadder, TennisLadderViewModel>().IgnoreAllNonExisting();
    }
  }
}
