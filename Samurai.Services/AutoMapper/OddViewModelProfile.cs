using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using AutoMapper;

using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Domain.Model;
using Samurai.Web.ViewModels.Value;

namespace Samurai.Services.AutoMapper
{
  public class OddViewModelProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<OddsForEvent, OddViewModel>()
            .IgnoreAllNonExisting();
      Mapper.CreateMap<GenericMatchCoupon, IEnumerable<OddViewModel>>()
            .ConvertUsing<OddViewModelConverter>();
      Mapper.CreateMap<IEnumerable<GenericMatchCoupon>, IEnumerable<OddViewModel>>()
            .ConvertUsing<OddViewModelConverterEnumerable>();
    }
  }

  public class OddViewModelConverterEnumerable : ITypeConverter<IEnumerable<GenericMatchCoupon>, IEnumerable<OddViewModel>>
  {
    public IEnumerable<OddViewModel> Convert(ResolutionContext context)
    {
      var ret = new List<OddViewModel>();

      var matchCoupons = (IEnumerable<GenericMatchCoupon>)context.SourceValue;

      foreach (var coupon in matchCoupons)
      {
        ret.AddRange(Mapper.Map<GenericMatchCoupon, IEnumerable<OddViewModel>>(coupon));
      }

      return ret;
    }
  }

  public class OddViewModelConverter : ITypeConverter<GenericMatchCoupon, IEnumerable<OddViewModel>>
  {
    public IEnumerable<OddViewModel> Convert(ResolutionContext context)
    {
      var ret = new List<OddViewModel>();
      var matchCoupon = (GenericMatchCoupon)context.SourceValue;
      foreach (var outcome in matchCoupon.ActualOdds.Keys)
      {
        var oddsForOutcome = matchCoupon.ActualOdds[outcome];
        var outcomeString = Regex.Replace(outcome.ToString(), "[a-z][A-Z]", m => m.Value[0] + " " + m.Value[1]);
        foreach (var odd in oddsForOutcome)
        {
          ret.Add(new OddViewModel
          {
            MatchId = matchCoupon.MatchId,
            IsBetable = true,
            Outcome = outcomeString,
            OddBeforeCommission = odd.OddsBeforeCommission,
            CommissionPct = odd.CommissionPct == 0 ? new Nullable<double>() : odd.CommissionPct,
            DecimalOdd = odd.DecimalOdds,
            Bookmaker = odd.BookmakerName,
            OddsSource = odd.Source,
            ClickThroughURL = odd.ClickThroughURL == null ? "" : odd.ClickThroughURL.ToString(),
            TimeStamp = odd.TimeStamp,
            Priority = odd.Priority
          });
        }
      }
      return ret;
    }
  }

}
