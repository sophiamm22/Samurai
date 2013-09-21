﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;

using Samurai.Domain.Entities;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;
using Samurai.Domain.Model;
using Samurai.Domain.Entities.ComplexTypes;

namespace Samurai.Services.AutoMapper
{
  public class FootballFixtureViewModelProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<GenericMatchDetail, FootballFixtureViewModel>()
        .IgnoreAllNonExisting()
        .ForMember(x => x.Id, opt => { opt.MapFrom(x => x.MatchID); })
        .ForMember(x => x.League, opt => { opt.MapFrom(x => x.TournamentName); })
        .ForMember(x => x.HomeTeam, opt => { opt.MapFrom(x => x.TeamOrPlayerA); })
        .ForMember(x => x.AwayTeam, opt => { opt.MapFrom(x => x.TeamOrPlayerB); })
        .ForMember(x => x.ScoreLine, opt => { opt.MapFrom(x => x.ObservedOutcome); });

      Mapper.CreateMap<FootballFixtureViewModel, FootballFixtureViewModel>().IgnoreAllNonExisting();

      Mapper.CreateMap<DaysFootballPredictions, FootballFixtureViewModel>()
        .ConvertUsing<FootballFixtureViewModelConverter>();
    }
  }

  public class FootballFixtureViewModelConverter : ITypeConverter<DaysFootballPredictions, FootballFixtureViewModel>
  {
    public FootballFixtureViewModel Convert(ResolutionContext context)
    {
      var prediction = (DaysFootballPredictions)context.SourceValue;
      var ret = new FootballFixtureViewModel()
      {
        Id = prediction.MatchID_pk,
        MatchIdentifier = prediction.MatchID_pk.ToString(),
        League = prediction.TournamentName,
        MatchDate = prediction.MatchDate,
        HomeTeam = prediction.TeamA,
        AwayTeam = prediction.TeamB,
        ScoreLine = prediction.Score
      };

      ret.Predictions = new FootballPredictionViewModel()
      {
        MatchId = prediction.MatchID_pk,
        MatchIdentifier = prediction.MatchID_pk.ToString(),
        Probabilities = new Dictionary<string, double>()
      };

      ret.Predictions.Probabilities.Add("HomeWin", (double)prediction.TeamAProbability);
      ret.Predictions.Probabilities.Add("Draw", (double)prediction.Draw);
      ret.Predictions.Probabilities.Add("AwayWin", (double)prediction.TeamBProbability);

      return ret;
    }
  }

}
