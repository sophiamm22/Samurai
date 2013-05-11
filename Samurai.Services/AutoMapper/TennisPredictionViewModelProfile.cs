﻿using System;
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
      
      Mapper.CreateMap<FootballPrediction, TennisPredictionViewModel>().ForMember(x => x.MatchId, opt =>
      { opt.MapFrom(x => x.MatchID); });

      Mapper.CreateMap<TennisPrediction, TennisPredictionViewModel>().ForMember(x => x.Probabilities, opt =>
        { opt.ResolveUsing<TennisProbabilityResolver>(); });

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

  public class TennisProbabilityResolver : ValueResolver<TennisPrediction, Dictionary<string, double>>
  {
    protected override Dictionary<string, double> ResolveCore(TennisPrediction source)
    {
      var ret = new Dictionary<string, double>();
      foreach (var outcomeKVP in source.OutcomeProbabilities)
      {
        ret.Add(outcomeKVP.Key.ToString(), outcomeKVP.Value);
      }
      return ret;
    }
  }
}
