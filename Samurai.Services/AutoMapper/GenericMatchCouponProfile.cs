using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;

using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Domain.Model;

namespace Samurai.Services.AutoMapper
{
  public class GenericMatchCouponProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<GenericMatchCoupon, GenericMatchCoupon>().IgnoreAllNonExisting();
      Mapper.CreateMap<GenericMatchCoupon, GenericMatchCoupon>().ForMember(x => x.HeadlineOdds, opt => opt.Ignore());
      Mapper.CreateMap<GenericMatchCoupon, GenericMatchCoupon>().ForMember(x => x.ActualOdds, opt => opt.Ignore());
    }
  }
}
