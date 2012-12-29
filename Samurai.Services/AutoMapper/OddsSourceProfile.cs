using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Domain.Entities;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Value;

namespace Samurai.Services.AutoMapper
{
  public class OddsSourceProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<ExternalSource, OddsSourceViewModel>().IgnoreAllNonExisting();
    }
  }
}
