using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;

using Samurai.Domain.Entities;
using Samurai.Domain.Model;
using Samurai.Web.ViewModels.Tennis;
using Samurai.Web.ViewModels.Value;

namespace Samurai.Services.AutoMapper
{
  public class TennisPredictionProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<TennisPredictionStat, TennisPrediction>().IgnoreAllNonExisting();
      Mapper.CreateMap<TennisPredictionStat, TennisPrediction>().ForMember(x => x.EPoints, opt =>
        opt.MapFrom(x => (double?)x.EPoints));
      Mapper.CreateMap<TennisPredictionStat, TennisPrediction>().ForMember(x => x.EGames, opt =>
        opt.MapFrom(x => (double?)x.EGames));
      Mapper.CreateMap<TennisPredictionStat, TennisPrediction>().ForMember(x => x.ESets, opt =>
        opt.MapFrom(x => (double?)x.ESets));
      
    }
  }
}
