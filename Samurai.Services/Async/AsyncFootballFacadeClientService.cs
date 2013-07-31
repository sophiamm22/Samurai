﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Core;
using Samurai.Services.Contracts.Async;
using Samurai.Web.ViewModels.Football;
using Samurai.Web.ViewModels.Value;

namespace Samurai.Services.Async
{
  public class AsyncFootballFacadeClientService : IAsyncFootballFacadeClientService
  {
    protected readonly IAsyncFootballFixtureService footballFixtureService;
    protected readonly IAsyncFootballPredictionService footballPredictionService;
    protected readonly IAsyncFootballOddsService footballOddsService;

    public AsyncFootballFacadeClientService(IAsyncFootballFixtureService footballFixtureService,
      IAsyncFootballPredictionService footballPredictionService, IAsyncFootballOddsService footballOddsService)
    {
      if (footballFixtureService == null) throw new ArgumentNullException("tennisFixtureService");
      if (footballPredictionService == null) throw new ArgumentNullException("tennisPredictionService");
      if (footballOddsService == null) throw new ArgumentNullException("tennisOddsService");

      this.footballFixtureService = footballFixtureService;
      this.footballPredictionService = footballPredictionService;
      this.footballOddsService = footballOddsService;
    }

    public async Task<IEnumerable<FootballFixtureViewModel>> GetDaysSchedule(DateTime fixtureDate)
    {
      var fixtures = await Task.Run(() =>
        this.footballFixtureService
            .GetFootballPredictions(fixtureDate));

      return fixtures;

      //var ret = new List<FootballFixtureViewModel>();

      //var fixtures = await Task.Run(() =>
      //  this.footballFixtureService
      //      .GetFootballFixturesByDate(fixtureDate));

      //var predictions = (await Task.Run(() =>
      //  this.footballPredictionService
      //      .GetFootballPredictionsNoScorePredictions(fixtures)))
      //      .ToDictionary(x => x.MatchIdentifier);

      //foreach (var fixture in fixtures)
      //{
      //  if (predictions.ContainsKey(fixture.MatchIdentifier))
      //  {
      //    fixture.Predictions = predictions[fixture.MatchIdentifier];
      //    fixture.Predictions.MatchId = fixture.Id;
      //  }
      //  ret.Add(fixture);
      //}
      //return ret;
    }

    public async Task<IEnumerable<OddViewModel>> GetDaysOdds(DateTime fixtureDate)
    {
      return await
        this.footballOddsService
            .GetDaysBestOdds(fixtureDate);
      
      //return await 
      //  this.footballOddsService
      //      .GetAllFootballTodaysOdds(fixtureDate);
    }

    public DateTime GetLatestDate()
    {
      return this.footballFixtureService.GetLatestDate();
    }
  }
}
