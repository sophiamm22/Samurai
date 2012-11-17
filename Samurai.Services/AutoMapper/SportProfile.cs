using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Domain.Entities;
using Samurai.Web.ViewModels;

namespace Samurai.Services.AutoMapper
{
  public class SportProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<Sport, SportViewModel>().IgnoreAllNonExisting();
    }
  }
}
