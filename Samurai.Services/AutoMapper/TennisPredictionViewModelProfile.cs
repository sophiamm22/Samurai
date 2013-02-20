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
  public class TennisPredictionViewModelProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<TennisPrediction, TennisPredictionViewModel>().IgnoreAllNonExisting();
      Mapper.CreateMap<TennisPrediction, TennisPredictionViewModel>().ForMember(x => x.PlayerAProbability, opt =>
        { opt.ResolveUsing<TennisProbabilityResolver>().ConstructedBy(() => new TennisProbabilityResolver(Outcome.HomeWin)); });
      Mapper.CreateMap<TennisPrediction, TennisPredictionViewModel>().ForMember(x => x.PlayerBProbability, opt =>
        { opt.ResolveUsing<TennisProbabilityResolver>().ConstructedBy(() => new TennisProbabilityResolver(Outcome.AwayWin)); });
      Mapper.CreateMap<TennisPrediction, TennisPredictionViewModel>().ForMember(x => x.ScoreLineProbabilities, opt =>
        { opt.ResolveUsing<TennisScoreLineResolver>(); });

    }
  }

  public class TennisScoreLineResolver : ValueResolver<TennisPrediction, IEnumerable<ScoreLineProbabilityViewModel>>
  {
    protected override IEnumerable<ScoreLineProbabilityViewModel> ResolveCore(TennisPrediction source)
    {
      if (source.ScoreLineProbabilities.Any(x => !x.Value.HasValue))
        return Enumerable.Empty<ScoreLineProbabilityViewModel>();
      else
        return source.ScoreLineProbabilities.Select(s => new ScoreLineProbabilityViewModel() { ScoreLine = s.Key, ScoreLineProbability = s.Value });
    }
  }

  public class TennisProbabilityResolver : ValueResolver<TennisPrediction, OutcomeProbabilityViewModel>
  {
    private readonly Outcome outcome;

    public TennisProbabilityResolver(Outcome outcome)
    {
      this.outcome = outcome;
    }

    protected override OutcomeProbabilityViewModel ResolveCore(TennisPrediction source)
    {
      var probability = source.OutcomeProbabilities.Count() == 0 ? 0.0 : source.OutcomeProbabilities[this.outcome];
      return new OutcomeProbabilityViewModel
      {
        Outcome = this.outcome.ToString(),
        OutcomeProbability = probability
      };
    }
  }
}
