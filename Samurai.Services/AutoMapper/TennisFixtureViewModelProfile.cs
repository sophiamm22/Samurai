﻿using System;
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
  public class TennisFixtureViewModelProfile : Profile
  {
    protected override void Configure()
    {
      Mapper.CreateMap<TennisMatchDetail, TennisFixtureViewModel>().IgnoreAllNonExisting();
      Mapper.CreateMap<TennisMatchDetail, TennisFixtureViewModel>().ForMember(x => x.Id, opt =>
        { opt.MapFrom(x => x.MatchID); });
      Mapper.CreateMap<TennisMatchDetail, TennisFixtureViewModel>().ForMember(x => x.Tournament, opt =>
        opt.MapFrom(x => x.TournamentName));
      Mapper.CreateMap<TennisMatchDetail, TennisFixtureViewModel>().ForMember(x => x.Year, opt =>
        opt.MapFrom(x => x.MatchDate.AddDays(7).Year));
      Mapper.CreateMap<TennisMatchDetail, TennisFixtureViewModel>().ForMember(x => x.PlayerASurname, opt =>
        opt.MapFrom(x => x.TeamOrPlayerA));
      Mapper.CreateMap<TennisMatchDetail, TennisFixtureViewModel>().ForMember(x => x.PlayerBSurname, opt =>
        opt.MapFrom(x => x.TeamOrPlayerB));
      Mapper.CreateMap<TennisMatchDetail, TennisFixtureViewModel>().ForMember(x => x.Predictions, opt =>
        opt.MapFrom(x => Mapper.Map<TennisPrediction, TennisPredictionViewModel>(x.TennisPrediction)));
      Mapper.CreateMap<TennisMatchDetail, TennisFixtureViewModel>().ForMember(x => x.ScoreLine, opt =>
        { opt.MapFrom(x => x.ObservedOutcome); });

      Mapper.CreateMap<GenericMatchDetail, TennisFixtureViewModel>().IgnoreAllNonExisting();
      Mapper.CreateMap<GenericMatchDetail, TennisFixtureViewModel>().ForMember(x => x.ScoreLine, opt =>
        { opt.MapFrom(x => x.ObservedOutcome); });

      Mapper.CreateMap<TennisFixtureViewModel, TennisFixtureViewModel>().IgnoreAllNonExisting();

      Mapper.CreateMap<DaysTennisPredictions, TennisFixtureViewModel>()
        .ConvertUsing<TennisFixtureViewModelConverter>();
    }
  }

  public class TennisFixtureViewModelConverter : ITypeConverter<DaysTennisPredictions, TennisFixtureViewModel>
  {
    public TennisFixtureViewModel Convert(ResolutionContext context)
    {
      var prediction = (DaysTennisPredictions)context.SourceValue;
      var ret = new TennisFixtureViewModel()
      {
        Id = prediction.MatchID_pk,
        MatchIdentifier = prediction.MatchID_pk.ToString(),
        Tournament = prediction.TournamentName,
        Year = prediction.MatchDate.AddDays(7).Year,
        PlayerAFirstName = prediction.PlayerBFirstName,
        PlayerASurname = prediction.PlayerASurname,
        PlayerBFirstName = prediction.PlayerBFirstName,
        PlayerBSurname = prediction.PlayerBSurname,
        ScoreLine = prediction.Score,
        MatchDate = prediction.MatchDate,
        Predictions = new TennisPredictionViewModel()
      };

      ret.Predictions.PlayerAGames = prediction.PlayerAGames;
      ret.Predictions.PlayerBGames = prediction.PlayerBGames;
      ret.Predictions.MatchDate = prediction.MatchDate;
      ret.Predictions.MatchIdentifier = prediction.MatchID_pk.ToString();
      ret.Predictions.MatchId = prediction.MatchID_pk;

      ret.Predictions.ESets = (double?)prediction.ESets;
      ret.Predictions.EGames = (double?)prediction.EGames;
      ret.Predictions.EPoints = (double?)prediction.EPoints;
      ret.Predictions.Probabilities = new Dictionary<string, double>()
      {
        { "HomeWin", (double)prediction.PlayerAProbability },
        { "AwayWin", (double)prediction.PlayerBProbability }
      };

      var scoreLineProbabilities = new List<ScoreLineProbabilityViewModel>();
      if (prediction.Score_2_0 != null)
        scoreLineProbabilities.Add(new ScoreLineProbabilityViewModel() { ScoreLine = "2-0", ScoreLineProbability = (double?)prediction.Score_2_0 });
      if (prediction.Score_2_1 != null)
        scoreLineProbabilities.Add(new ScoreLineProbabilityViewModel() { ScoreLine = "2-1", ScoreLineProbability = (double?)prediction.Score_2_1 });
      if (prediction.Score_1_2 != null)
        scoreLineProbabilities.Add(new ScoreLineProbabilityViewModel() { ScoreLine = "1-2", ScoreLineProbability = (double?)prediction.Score_1_2 });
      if (prediction.Score_0_2 != null)
        scoreLineProbabilities.Add(new ScoreLineProbabilityViewModel() { ScoreLine = "0-2", ScoreLineProbability = (double?)prediction.Score_0_2 });

      if (prediction.Score_3_0 != null)
        scoreLineProbabilities.Add(new ScoreLineProbabilityViewModel() { ScoreLine = "3-0", ScoreLineProbability = (double?)prediction.Score_3_0 });
      if (prediction.Score_3_1 != null)
        scoreLineProbabilities.Add(new ScoreLineProbabilityViewModel() { ScoreLine = "3-1", ScoreLineProbability = (double?)prediction.Score_3_1 });
      if (prediction.Score_3_2 != null)
        scoreLineProbabilities.Add(new ScoreLineProbabilityViewModel() { ScoreLine = "3-2", ScoreLineProbability = (double?)prediction.Score_3_2 });
      if (prediction.Score_2_3 != null)
        scoreLineProbabilities.Add(new ScoreLineProbabilityViewModel() { ScoreLine = "2-3", ScoreLineProbability = (double?)prediction.Score_2_3 });
      if (prediction.Score_1_3 != null)
        scoreLineProbabilities.Add(new ScoreLineProbabilityViewModel() { ScoreLine = "1-3", ScoreLineProbability = (double?)prediction.Score_1_3 });
      if (prediction.Score_0_3 != null)
        scoreLineProbabilities.Add(new ScoreLineProbabilityViewModel() { ScoreLine = "0-3", ScoreLineProbability = (double?)prediction.Score_0_3 });

      ret.Predictions.ScoreLineProbabilities = scoreLineProbabilities;
      
      return ret;
    }
  }
}
