using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;

using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Web.ViewModels.Value;

namespace Samurai.Services.AutoMapper
{
  public class OddViewModelProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<OddsForEvent, OddViewModel>()
            .IgnoreAllNonExisting();
    }
  }
}
