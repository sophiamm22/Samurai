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
  public class FootballCouponDictionaryProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<List<FootballCouponOutcomeViewModel>, FootballCouponOutcomeViewModel>().IgnoreAllNonExisting();
      Mapper.CreateMap<List<FootballCouponOutcomeViewModel>, FootballCouponOutcomeViewModel>().ForMember(x => x.MatchIdentifier,
        x => x.MapFrom(opt => opt.First()));
      Mapper.CreateMap<List<FootballCouponOutcomeViewModel>, FootballCouponOutcomeViewModel>().ForMember(x => x.CouponURL, opt =>
        { opt.ResolveUsing<FootballCouponURLDictionaryResolver>(); });
      Mapper.CreateMap<List<FootballCouponOutcomeViewModel>, FootballCouponOutcomeViewModel>().ForMember(x => x.OddsCollection, opt =>
        { opt.ResolveUsing<FootballCouponListToSingleResolver>().ConstructedBy(() => new FootballCouponListToSingleResolver(Outcome.HomeWin)); });
      Mapper.CreateMap<List<FootballCouponOutcomeViewModel>, FootballCouponOutcomeViewModel>().ForMember(x => x.Draw, opt =>
        { opt.ResolveUsing<FootballCouponListToSingleResolver>().ConstructedBy(() => new FootballCouponListToSingleResolver(Outcome.Draw)); });
      Mapper.CreateMap<List<FootballCouponOutcomeViewModel>, FootballCouponOutcomeViewModel>().ForMember(x => x.AwayWin, opt =>
        { opt.ResolveUsing<FootballCouponListToSingleResolver>().ConstructedBy(() => new FootballCouponListToSingleResolver(Outcome.AwayWin)); });


    }
  }

  public class FootballCouponListToSingleResolver : ValueResolver<List<FootballCouponOutcomeViewModel>, IEnumerable<OddViewModel>>
  {
    private readonly Outcome outcome;
    public FootballCouponListToSingleResolver(Outcome outcome)
    {
      this.outcome = outcome;
    }

    protected override IEnumerable<OddViewModel> ResolveCore(List<FootballCouponOutcomeViewModel> source)
    {
      var ret = new List<OddViewModel>();
      if (this.outcome == Outcome.HomeWin)
        source.SelectMany(x => x.OddsCollection).ToList().ForEach(x => ret.Add(x));
      else if (this.outcome == Outcome.Draw)
        source.SelectMany(x => x.Draw).ToList().ForEach(x => ret.Add(x));
      else if (this.outcome == Outcome.AwayWin)
        source.SelectMany(x => x.AwayWin).ToList().ForEach(x => ret.Add(x));

      return ret;
    }
  }


  public class FootballCouponURLDictionaryResolver : ValueResolver<List<FootballCouponOutcomeViewModel>, Dictionary<string, string>>
  {
    protected override Dictionary<string, string> ResolveCore(List<FootballCouponOutcomeViewModel> source)
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
