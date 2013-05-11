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
  public class TennisCouponDictionaryProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<List<TennisCouponViewModel>, TennisCouponViewModel>().IgnoreAllNonExisting();
      Mapper.CreateMap<List<TennisCouponViewModel>, TennisCouponViewModel>().ForMember(x => x.MatchIdentifier,
        x => x.MapFrom(opt => opt.First()));
      Mapper.CreateMap<List<TennisCouponViewModel>, TennisCouponViewModel>().ForMember(x => x.CouponURL, opt =>
        { opt.ResolveUsing<TennisCouponURLDictionaryResolver>(); });
      Mapper.CreateMap<List<TennisCouponViewModel>, TennisCouponViewModel>().ForMember(x => x.PlayerAOdds, opt =>
        { opt.ResolveUsing<TennisCouponListToSingleResolver>().ConstructedBy(() => new TennisCouponListToSingleResolver(Outcome.HomeWin)); });
      Mapper.CreateMap<List<TennisCouponViewModel>, TennisCouponViewModel>().ForMember(x => x.PlayerBOdds, opt =>
        { opt.ResolveUsing<TennisCouponListToSingleResolver>().ConstructedBy(() => new TennisCouponListToSingleResolver(Outcome.AwayWin)); });
    }
  }

  public class TennisCouponListToSingleResolver : ValueResolver<List<TennisCouponViewModel>, IEnumerable<OddViewModel>>
  {
    private readonly Outcome outcome;
    public TennisCouponListToSingleResolver(Outcome outcome)
    {
      this.outcome = outcome;
    }

    protected override IEnumerable<OddViewModel> ResolveCore(List<TennisCouponViewModel> source)
    {
      var ret = new List<OddViewModel>();
      if (this.outcome == Outcome.HomeWin)
        source.SelectMany(x => x.PlayerAOdds).ToList().ForEach(x => ret.Add(x));
      else if (this.outcome == Outcome.AwayWin)
        source.SelectMany(x => x.PlayerBOdds).ToList().ForEach(x => ret.Add(x));

      return ret;
    }
  }


  public class TennisCouponURLDictionaryResolver : ValueResolver<List<TennisCouponViewModel>, Dictionary<string, string>>
  {
    protected override Dictionary<string, string> ResolveCore(List<TennisCouponViewModel> source)
    {
      var ret = new Dictionary<string, string>();
      foreach (var urlKVPs in source.Select(x => x.CouponURL))
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
