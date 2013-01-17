using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;

using Samurai.Domain.Entities;
using Samurai.Domain.Model;
using Samurai.Web.ViewModels.Tennis;
using Samurai.Web.ViewModels;

namespace Samurai.Services.AutoMapper
{
  public class TournamentEventViewModelProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<TournamentEvent, TournamentEventViewModel>().IgnoreAllNonExisting();
    }
  }
}
