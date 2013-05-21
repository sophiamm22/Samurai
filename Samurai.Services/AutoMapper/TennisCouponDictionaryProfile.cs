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
      //Mapper.CreateMap<List<TennisCouponOutcomeViewModel>, TennisCouponOutcomeViewModel>().IgnoreAllNonExisting();
      //Mapper.CreateMap<List<TennisCouponOutcomeViewModel>, TennisCouponOutcomeViewModel>().ForMember(x => x.MatchIdentifier,
      //  x => x.MapFrom(opt => opt.First()));
      //Mapper.CreateMap<List<TennisCouponOutcomeViewModel>, TennisCouponOutcomeViewModel>().ForMember(x => x.CouponURL, opt =>
      //  { opt.ResolveUsing<TennisCouponURLDictionaryResolver>(); });
      //Mapper.CreateMap<List<TennisCouponOutcomeViewModel>, TennisCouponOutcomeViewModel>().ForMember(x => x.OddsCollection, opt =>
      //  { opt.ResolveUsing<TennisCouponListToSingleResolver>().ConstructedBy(() => new TennisCouponListToSingleResolver(Outcome.HomeWin)); });
      //Mapper.CreateMap<List<TennisCouponOutcomeViewModel>, TennisCouponOutcomeViewModel>().ForMember(x => x.AwayWin, opt =>
      //  { opt.ResolveUsing<TennisCouponListToSingleResolver>().ConstructedBy(() => new TennisCouponListToSingleResolver(Outcome.AwayWin)); });
    }
  }

  public class TennisCouponListToSingleResolver : ValueResolver<List<TennisCouponOutcomeViewModel>, IEnumerable<OddViewModel>>
  {
    private readonly Outcome outcome;
    public TennisCouponListToSingleResolver(Outcome outcome)
    {
      this.outcome = outcome;
    }

    protected override IEnumerable<OddViewModel> ResolveCore(List<TennisCouponOutcomeViewModel> source)
    {
      var ret = new List<OddViewModel>();
      //if (this.outcome == Outcome.HomeWin)
      //  source.SelectMany(x => x.OddsCollection).ToList().ForEach(x => ret.Add(x));
      //else if (this.outcome == Outcome.AwayWin)
      //  source.SelectMany(x => x.AwayWin).ToList().ForEach(x => ret.Add(x));

      return ret;
    }
  }


  public class TennisCouponURLDictionaryResolver : ValueResolver<List<TennisCouponOutcomeViewModel>, Dictionary<string, string>>
  {
    protected override Dictionary<string, string> ResolveCore(List<TennisCouponOutcomeViewModel> source)
    {
      var ret = new Dictionary<string, string>();
      //foreach (var urlKVPs in source.Select(x => x.CouponURL))
      //{
      //  foreach (var urlKVP in urlKVPs)
      //  {
      //    if (!ret.ContainsKey(urlKVP.Key))
      //      ret.Add(urlKVP.Key, urlKVP.Value);
      //  }
      //}
      return ret;
    }
  }

}
