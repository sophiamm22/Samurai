﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;

using Samurai.Domain.Entities;
using Samurai.Domain.Model;
using Samurai.Web.ViewModels.Football;
using Samurai.Web.ViewModels.Value;

namespace Samurai.Services.AutoMapper
{
  public class FootballPredictionViewModelProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<FootballPrediction, FootballPredictionViewModel>().IgnoreAllNonExisting();

      Mapper.CreateMap<FootballPrediction, FootballPredictionViewModel>().ForMember(x => x.MatchId, opt =>
        { opt.MapFrom(x => x.MatchID); });
      
      Mapper.CreateMap<FootballPrediction, FootballPredictionViewModel>().ForMember(x => x.Probabilities, opt =>
        { opt.ResolveUsing<FootballProbabilityResolver>(); });

      Mapper.CreateMap<FootballPrediction, FootballPredictionViewModel>().ForMember(x => x.ScoreLineProbabilities, opt =>
        { opt.ResolveUsing<FootballScoreLineResolver>(); });
    }

    public class FootballScoreLineResolver : ValueResolver<FootballPrediction, IEnumerable<ScoreLineProbabilityViewModel>>
    {
      protected override IEnumerable<ScoreLineProbabilityViewModel> ResolveCore(FootballPrediction source)
      {
        if (source.ScoreLineProbabilities.Any(x => !x.Value.HasValue))
          return Enumerable.Empty<ScoreLineProbabilityViewModel>();
        else
          return source.ScoreLineProbabilities.Select(s => new ScoreLineProbabilityViewModel() { ScoreLine = s.Key, ScoreLineProbability = s.Value });
      }
    }

    public class FootballProbabilityResolver : ValueResolver<FootballPrediction, Dictionary<string, double>>
    {
      protected override Dictionary<string, double> ResolveCore(FootballPrediction source)
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
}
