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
using Samurai.Domain.Entities.ComplexTypes;

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

      Mapper.CreateMap<IEnumerable<OddsForEvent>, TennisCouponViewModel>().IgnoreAllNonExisting();
      //Mapper.CreateMap<IEnumerable<OddsForEvent>, TennisCouponViewModel>().ForMember(x => x, opt =>
      //  { opt.ResolveUsing<TennisCouponOddsForEventResolver>(); });
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

      var bestOddsAvailable = source.ActualOdds[this.outcome].Max(x => x.DecimalOdds);
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

  public class TennisCouponOddsForEventResolver : ValueResolver<IEnumerable<OddsForEvent>, TennisCouponViewModel>
  {
    protected override TennisCouponViewModel ResolveCore(IEnumerable<OddsForEvent> source)
    {
      var ret = new TennisCouponViewModel();

      var playerAWins =
        source.Where(x => x.Outcome == "Home Win")
              .ToList();
      var playerBWins =
        source.Where(x => x.Outcome == "Away Win")
              .ToList();

      ret.PlayerAOdds = ConvertToViewModel(playerAWins);
      ret.PlayerBOdds = ConvertToViewModel(playerBWins);
      return ret;
    }

    private IEnumerable<OddViewModel> ConvertToViewModel(IEnumerable<OddsForEvent> oddsForEvent)
    {
      var ret = new List<OddViewModel>();

      foreach (var odd in oddsForEvent)
      {
        var oddViewModel = new OddViewModel
        {
          Outcome = odd.Outcome,
          OddBeforeCommission = (double)odd.OddBeforeCommission,
          CommissionPct = odd.CommissionPct,
          DecimalOdd = odd.DecimalOdd,
          TimeStamp = odd.TimeStamp,
          Bookmaker = odd.Bookmaker,
          OddsSource = odd.OddsSource,
          ClickThroughURL = odd.ClickThroughURL,
          Priority = odd.Priority
        };
        ret.Add(oddViewModel);
      }

      return ret;
    }
  }
}
