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
  public class FootballCouponViewModelProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<GenericMatchCoupon, FootballCouponViewModel>().IgnoreAllNonExisting();
      Mapper.CreateMap<GenericMatchCoupon, FootballCouponViewModel>().ForMember(x => x.CouponURL, opt =>
        { opt.ResolveUsing<GenericMatchCouponURLDictionaryResolver>(); });
      Mapper.CreateMap<GenericMatchCoupon, FootballCouponViewModel>().ForMember(x => x.HomeOdds, opt =>
        { opt.ResolveUsing<FootballCouponOddsResolver>().ConstructedBy(() => new FootballCouponOddsResolver(Outcome.HomeWin)); });
      Mapper.CreateMap<GenericMatchCoupon, FootballCouponViewModel>().ForMember(x => x.DrawOdds, opt =>
        { opt.ResolveUsing<FootballCouponOddsResolver>().ConstructedBy(() => new FootballCouponOddsResolver(Outcome.Draw)); });
      Mapper.CreateMap<GenericMatchCoupon, FootballCouponViewModel>().ForMember(x => x.AwayOdds, opt =>
        { opt.ResolveUsing<FootballCouponOddsResolver>().ConstructedBy(() => new FootballCouponOddsResolver(Outcome.AwayWin)); });
    }
  }

  public class GenericMatchCouponURLDictionaryResolver : ValueResolver<GenericMatchCoupon, Dictionary<string, string>>
  {
    protected override Dictionary<string, string> ResolveCore(GenericMatchCoupon source)
    {
      var ret = new Dictionary<string, string>();

      ret.Add(source.Source, source.MatchURL.ToString());

      return ret;
    }
  }

  public class FootballCouponOddsResolver : ValueResolver<GenericMatchCoupon, IEnumerable<OddViewModel>>
  {
    private readonly Outcome outcome;

    public FootballCouponOddsResolver(Outcome outcome)
    {
      this.outcome = outcome;
    }

    protected override IEnumerable<OddViewModel> ResolveCore(GenericMatchCoupon source)
    {
      var ret = new List<OddViewModel>();

      var actualOutcome = this.outcome == Outcome.Draw ? "Draw" : (this.outcome == Outcome.HomeWin ? source.TeamOrPlayerA : source.TeamOrPlayerB);

      var bestOddsAvailable = source.HeadlineOdds.Count == 0 ? source.ActualOdds[this.outcome].Max(x => x.DecimalOdds) : source.HeadlineOdds[this.outcome];
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
        ClickThroughURL = x.ClickThroughURL == null ? null : x.ClickThroughURL.ToString(),
        Priority = x.Priority
      }));

      return ret;
    }
  }
}
