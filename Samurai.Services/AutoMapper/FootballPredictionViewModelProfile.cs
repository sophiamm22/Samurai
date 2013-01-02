using System;
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
      Mapper.CreateMap<FootballPrediction, FootballPredictionViewModel>().ForMember(x => x.HomeWinProbability, opt =>
        { opt.ResolveUsing<FootballProbabilityResolver>().ConstructedBy(() => new FootballProbabilityResolver(Outcome.HomeWin)); });
      Mapper.CreateMap<FootballPrediction, FootballPredictionViewModel>().ForMember(x => x.DrawProbabilitity, opt =>
        { opt.ResolveUsing<FootballProbabilityResolver>().ConstructedBy(() => new FootballProbabilityResolver(Outcome.Draw)); });
      Mapper.CreateMap<FootballPrediction, FootballPredictionViewModel>().ForMember(x => x.AwayWinProbability, opt =>
        { opt.ResolveUsing<FootballProbabilityResolver>().ConstructedBy(() => new FootballProbabilityResolver(Outcome.AwayWin)); });
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

    public class FootballProbabilityResolver : ValueResolver<FootballPrediction, OutcomeProbabilityViewModel>
    {
      private readonly Outcome outcome;

      public FootballProbabilityResolver(Outcome outcome)
      {
        this.outcome = outcome;
      }

      protected override OutcomeProbabilityViewModel ResolveCore(FootballPrediction source)
      {
        var probability = source.OutcomeProbabilities[this.outcome];
        return new OutcomeProbabilityViewModel
        {
          Outcome = this.outcome.ToString(),
          OutcomeProbability = probability
        };
      }
    }
  }
}
