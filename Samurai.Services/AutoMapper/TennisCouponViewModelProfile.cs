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

namespace Samurai.Services.AutoMapper
{
  public class TennisCouponViewModelProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<GenericMatchCoupon, TennisCouponViewModel>().IgnoreAllNonExisting();
      Mapper.CreateMap<GenericMatchCoupon, TennisCouponViewModel>().ForMember(x => x.CouponURL, opt =>
        { opt.ResolveUsing<GenericMatchCouponURLDictionaryResolver>(); });
      Mapper.CreateMap<GenericMatchCoupon, TennisCouponViewModel>().ForMember(x => x.PlayerAOdds, opt =>
        { opt.ResolveUsing<TennisCouponOddsResolver>().ConstructedBy(() => new TennisCouponOddsResolver(Outcome.HomeWin)); });
      Mapper.CreateMap<GenericMatchCoupon, TennisCouponViewModel>().ForMember(x => x.PlayerBOdds, opt =>
        { opt.ResolveUsing<TennisCouponOddsResolver>().ConstructedBy(() => new TennisCouponOddsResolver(Outcome.AwayWin)); });
    }
  }

  public class TennisCouponOddsResolver : ValueResolver<GenericMatchCoupon, IEnumerable<OddViewModel>>
  {
    private readonly Outcome outcome;

    public TennisCouponOddsResolver(Outcome outcome)
    {
      this.outcome = outcome;
    }

    protected override IEnumerable<OddViewModel> ResolveCore(GenericMatchCoupon source)
    {
      var ret = new List<OddViewModel>();

      var actualOutcome = this.outcome == Outcome.Draw ? "Draw" : (this.outcome == Outcome.HomeWin ? source.TeamOrPlayerA : source.TeamOrPlayerB);

      var bestOddsAvailable = source.HeadlineOdds[this.outcome];
      ret.Add(new OddViewModel
      {
        IsBetable = false,
        Outcome = actualOutcome,
        OddBeforeCommission = bestOddsAvailable,
        CommissionPct = 0,
        DecimalOdd = bestOddsAvailable,
        TimeStamp = source.LastChecked,
        Bookmaker = string.Format("{0} Best Available", source.Source),
        OddsSource = source.Source,
        ClickThroughURL = source.MatchURL.ToString(),
        Priority = 10000
      });

      var oddsForOutcome = source.ActualOdds[this.outcome];
      oddsForOutcome.ToList().ForEach(x =>
        ret.Add(new OddViewModel
        {
          IsBetable = true,
          Outcome = actualOutcome,
          OddBeforeCommission = x.OddsBeforeCommission,
          CommissionPct = x.CommissionPct,
          DecimalOdd = x.DecimalOdds,
          TimeStamp = x.TimeStamp,
          Bookmaker = x.BookmakerName,
          OddsSource = x.Source,
          ClickThroughURL = x.ClickThroughURL.ToString(),
          Priority = x.Priority
        }));

      return ret;
    }
  }
}
