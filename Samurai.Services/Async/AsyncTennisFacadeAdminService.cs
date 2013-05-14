﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Core;
using Samurai.Services.Contracts.Async;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Tennis;
using Samurai.Web.ViewModels.Value;
using Samurai.Domain.Model;
using Samurai.Domain.Infrastructure;

namespace Samurai.Services.Async
{
  public class AsyncTennisFacadeAdminService : IAsyncTennisFacadeAdminService
  {
    protected readonly IAsyncTennisFixtureService tennisFixtureService;
    protected readonly IAsyncTennisPredictionService tennisPredictionService;
    protected readonly IAsyncTennisOddsService tennisOddsService;

    public AsyncTennisFacadeAdminService(IAsyncTennisFixtureService tennisFixtureService,
      IAsyncTennisPredictionService tennisPredictionService, IAsyncTennisOddsService tennisOddsService)
    {
      if (tennisFixtureService == null) throw new ArgumentNullException("tennisFixtureService");
      if (tennisPredictionService == null) throw new ArgumentNullException("tennisPredictionService");
      if (tennisOddsService == null) throw new ArgumentNullException("tennisOddsService");

      this.tennisFixtureService = tennisFixtureService;
      this.tennisPredictionService = tennisPredictionService;
      this.tennisOddsService = tennisOddsService;
    }

    //The admin version will get all odds too
    public async Task<IEnumerable<TennisFixtureViewModel>> GetDaysSchedule(DateTime fixtureDate)
    {
      ProgressReporterProvider.Current.ReportProgress(string.Format("Getting Days Tennis Schedule for {0}", fixtureDate.ToShortDateString()), ReporterImportance.High, ReporterAudience.Admin);

      var ret = new List<TennisFixtureViewModel>();
      var groupedCoupons = new Dictionary<string, List<TennisCouponViewModel>>();

      var tennisFixtures = new List<TennisFixtureViewModel>();
      tennisFixtures.AddRange(await this.tennisPredictionService.GetTennisPredictions(fixtureDate));

      var tennisOdds = await
        this.tennisOddsService
            .GetAllTennisOdds(tennisFixtures.Select(x => x.ID).ToList());

      foreach (var coupon in tennisOdds)
      {
        if (!groupedCoupons.ContainsKey(coupon.MatchIdentifier))
          groupedCoupons.Add(coupon.MatchIdentifier, new List<TennisCouponViewModel>());
        groupedCoupons[coupon.MatchIdentifier].Add(coupon);
      }

      var flatCoupons = Mapper.Map<Dictionary<string, List<TennisCouponViewModel>>, Dictionary<string, TennisCouponViewModel>>(groupedCoupons);

      foreach (var tennisFixture in tennisFixtures)
      {
        TennisCouponViewModel oddsDecider;
        oddsDecider = flatCoupons.ContainsKey(tennisFixture.MatchIdentifier) ? flatCoupons[tennisFixture.MatchIdentifier] : null;
        ret.Add(TennisFixtureViewModel.CreateCombination(tennisFixture, oddsDecider));
      }
      return ret;
    }

    public async Task<IEnumerable<TennisFixtureViewModel>> UpdateDaysSchedule(DateTime fixtureDate)
    {
      ProgressReporterProvider.Current.ReportProgress(string.Format("Updating Days Tennis Schedule for {0}", fixtureDate.ToShortDateString()), ReporterImportance.High, ReporterAudience.Admin);

      var ret = new List<TennisFixtureViewModel>();

      var tennisFixturesAndPredictions = await
        UpdateDaysFixturesAndPredicitons(fixtureDate);

      var tennisOdds = await 
        UpdateDaysOdds(fixtureDate);

      foreach (var tennisFixture in tennisFixturesAndPredictions)
      {
        TennisCouponViewModel oddsDecider;

        oddsDecider = tennisOdds.ContainsKey(tennisFixture.MatchIdentifier) ? tennisOdds[tennisFixture.MatchIdentifier] : null;
        var tennisFixtureViewModel = TennisFixtureViewModel.CreateCombination(tennisFixture, oddsDecider);
        ret.Add(tennisFixtureViewModel);
      }

      return ret;
    }

    public async Task<IEnumerable<TournamentEventViewModel>> FetchTournamentEvents()
    {
      ProgressReporterProvider.Current.ReportProgress("Getting Tournament Events From Tennis Betting 365", ReporterImportance.High, ReporterAudience.Admin);

      return await 
        this.tennisFixtureService
            .FetchTournamentEvents();
    }

    public void AddTournamentCouponURL(TournamentCouponURLViewModel viewModel)
    {
      ProgressReporterProvider.Current.ReportProgress(string.Format("Adding tournament coupon for {0}", viewModel.Tournament), ReporterImportance.High, ReporterAudience.Admin);

      this.tennisOddsService.AddTournamentCouponURL(viewModel);
    }

    public async Task<IEnumerable<TennisLadderViewModel>> FetchTournamentLadder(DateTime matchDate, string tournament)
    {
      return await 
        this.tennisFixtureService
            .FetchTournamentLadder(matchDate, tournament);
    }

    public void AddAlias(string source, string playerName, string valueSamuraiName, string valueSamuraiFirstName)
    {
      this.tennisFixtureService.AddAlias(source, playerName, valueSamuraiName, valueSamuraiFirstName);
    }

    private async Task<IList<ShowTournamentLadderChallengeViewModel>> CalculateRoundTournamentLadderChallenge(IList<ShowTournamentLadderChallengeViewModel> seed,
      string tournamentSlug, int year, int round)
    {
      var previousRoundMatches
        = seed.Where(x => x.RoundNumber == round - 1)
              .ToList();

      if (previousRoundMatches.Count() <= 1)
        return seed;

      for (int i = 0; i < previousRoundMatches.Count; i += 2)
      {
        var viewModel = new ShowTournamentLadderChallengeViewModel() { RoundNumber = round };
        int shift = 0;
        int loserShift = 0;

        if (previousRoundMatches[i + 1].ExpectedWinnerSurname == "Bye")
        {
          shift = i;
          loserShift = i + 1;
          viewModel.Probability = 1;
        }
        else if (previousRoundMatches[i].ExpectedWinnerSurname == "Bye")
        {
          shift = i + 1;
          loserShift = i;
          viewModel.Probability = 1;
        }
        else
        {
          bool playerAGoesThrough = true;
          var prediction = await 
            this.tennisPredictionService.GetSingleTennisPrediction(
              previousRoundMatches[i].ExpectedWinnerSurname,
              previousRoundMatches[i].ExpectedWinnerFirstName,
              previousRoundMatches[i + 1].ExpectedWinnerSurname,
              previousRoundMatches[i + 1].ExpectedWinnerFirstName,
              year,
              tournamentSlug,
              false /*we don't want to accidentally introduce hindsight*/);

          if (prediction.Predictions.Probabilities["homeWin"] > prediction.Predictions.Probabilities["awayWin"])
            playerAGoesThrough = true;
          else if (prediction.Predictions.Probabilities["homeWin"] < prediction.Predictions.Probabilities["awayWin"])
            playerAGoesThrough = false;
          else
            playerAGoesThrough = prediction.Predictions.PlayerAGames >= prediction.Predictions.PlayerBGames;

          shift = playerAGoesThrough ? i : (i + 1);
          loserShift = playerAGoesThrough ? (i + 1) : i;
          viewModel.Probability = playerAGoesThrough ? prediction.Predictions.Probabilities["homeWin"] : prediction.Predictions.Probabilities["awayWin"];
        }
        viewModel.ExpectedWinner = previousRoundMatches[shift].ExpectedWinner;
        viewModel.ExpectedWinnerFirstName = previousRoundMatches[shift].ExpectedWinnerFirstName;
        viewModel.ExpectedWinnerSurname = previousRoundMatches[shift].ExpectedWinnerSurname;
        viewModel.ExpectedLoser = previousRoundMatches[loserShift].ExpectedWinner;
        viewModel.ExpectedLoserFirstName = previousRoundMatches[loserShift].ExpectedLoserFirstName;
        viewModel.ExpectedLoserSurname = previousRoundMatches[loserShift].ExpectedLoserSurname;

        seed.Add(viewModel);
      }


      return await CalculateRoundTournamentLadderChallenge(seed, tournamentSlug, year, round + 1);
    }

    private async Task<IEnumerable<TennisFixtureViewModel>> UpdateDaysFixturesAndPredicitons(DateTime fixtureDate)
    {
      var tennisFixtures = new List<TennisFixtureViewModel>();
      var daysMatchCount = this.tennisPredictionService.GetCountOfDaysPredictions(fixtureDate, "Tennis");
      if (daysMatchCount == 0)
        tennisFixtures.AddRange(await this.tennisPredictionService.FetchTennisPredictions(fixtureDate));
      else
      {
        var daysPredictionCoupons = await this.tennisPredictionService.FetchTennisPredictionCoupons(fixtureDate);
        if (daysMatchCount == daysPredictionCoupons.Count())
        {
          tennisFixtures.AddRange(await this.tennisPredictionService.GetTennisPredictions(fixtureDate));
        }
        else
        {
          tennisFixtures.AddRange(await this.tennisPredictionService.FetchTennisPredictions(fixtureDate));
        }
      }

      if (tennisFixtures.Count == 0)
        return Enumerable.Empty<TennisFixtureViewModel>();

      return tennisFixtures;
    }

    private async Task<Dictionary<string, TennisCouponViewModel>> UpdateDaysOdds(DateTime matchDate)
    {
      var groupedCoupons = new Dictionary<string, List<TennisCouponViewModel>>();

      var daysCoupons = await
        this.tennisOddsService
            .FetchAllTennisOdds(matchDate);

      foreach (var coupon in daysCoupons)
      {
        if (!groupedCoupons.ContainsKey(coupon.MatchIdentifier))
          groupedCoupons.Add(coupon.MatchIdentifier, new List<TennisCouponViewModel>());
        groupedCoupons[coupon.MatchIdentifier].Add(coupon);
      }

      var ret = Mapper.Map<Dictionary<string, List<TennisCouponViewModel>>, Dictionary<string, TennisCouponViewModel>>(groupedCoupons);

      return ret;
    }
  }
}
