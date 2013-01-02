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
  public class FootballCouponDictionary : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<List<FootballCouponViewModel>, FootballCouponViewModel>().IgnoreAllNonExisting();
      Mapper.CreateMap<List<FootballCouponViewModel>, FootballCouponViewModel>().ForMember(x => x.MatchIdentifier,
        x => x.MapFrom(opt => opt.First()));
      Mapper.CreateMap<List<FootballCouponViewModel>, FootballCouponViewModel>().ForMember(x => x.CouponURL, opt =>
        { opt.ResolveUsing<FootballCouponURLDictionaryResolver>(); });
      Mapper.CreateMap<List<FootballCouponViewModel>, FootballCouponViewModel>().ForMember(x => x.HomeOdds, opt =>
        { opt.ResolveUsing<FootballCouponListToSingleResolver>().ConstructedBy(() => new FootballCouponListToSingleResolver(Outcome.HomeWin)); });
      Mapper.CreateMap<List<FootballCouponViewModel>, FootballCouponViewModel>().ForMember(x => x.DrawOdds, opt =>
        { opt.ResolveUsing<FootballCouponListToSingleResolver>().ConstructedBy(() => new FootballCouponListToSingleResolver(Outcome.Draw)); });
      Mapper.CreateMap<List<FootballCouponViewModel>, FootballCouponViewModel>().ForMember(x => x.AwayOdds, opt =>
        { opt.ResolveUsing<FootballCouponListToSingleResolver>().ConstructedBy(() => new FootballCouponListToSingleResolver(Outcome.AwayWin)); });


    }
  }

  public class FootballCouponListToSingleResolver : ValueResolver<List<FootballCouponViewModel>, IEnumerable<OddViewModel>>
  {
    private readonly Outcome outcome;
    public FootballCouponListToSingleResolver(Outcome outcome)
    {
      this.outcome = outcome;
    }

    protected override IEnumerable<OddViewModel> ResolveCore(List<FootballCouponViewModel> source)
    {
      var ret = new List<OddViewModel>();
      if (this.outcome == Outcome.HomeWin)
        source.SelectMany(x => x.HomeOdds).ToList().ForEach(x => ret.Add(x));
      else if (this.outcome == Outcome.Draw)
        source.SelectMany(x => x.DrawOdds).ToList().ForEach(x => ret.Add(x));
      else if (this.outcome == Outcome.AwayWin)
        source.SelectMany(x => x.AwayOdds).ToList().ForEach(x => ret.Add(x));

      return ret;
    }
  }


  public class FootballCouponURLDictionaryResolver : ValueResolver<List<FootballCouponViewModel>, Dictionary<string, string>>
  {
    protected override Dictionary<string, string> ResolveCore(List<FootballCouponViewModel> source)
    {
      var ret = new Dictionary<string, string>();
      foreach (var urlKVPs in source.Select(x=>x.CouponURL))
      {
        foreach (var urlKVP in urlKVPs)
        {
          if (!ret.ContainsKey(urlKVP.Key))
            ret.Add(urlKVP.Key, urlKVP.Value);
        }
      }
      return ret;
    }
  }
}
