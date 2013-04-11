using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Core;
using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Tennis;
using Samurai.Web.ViewModels.Value;
using Samurai.Domain.Model;
using Samurai.Domain.Infrastructure;

namespace Samurai.Services.AdminServices
{
  public class TennisFacadeAdminService : ITennisFacadeAdminService
  {
    protected readonly ITennisFixtureAdminService tennisFixtureService;
    protected readonly ITennisPredictionAdminService tennisPredictionService;
    protected readonly ITennisOddsAdminService tennisOddsService;

    public TennisFacadeAdminService(ITennisFixtureAdminService tennisFixtureService,
      ITennisPredictionAdminService tennisPredictionService, ITennisOddsAdminService tennisOddsService)
    {
      if (tennisFixtureService == null) throw new ArgumentNullException("tennisFixtureService");
      if (tennisPredictionService == null) throw new ArgumentNullException("tennisPredictionService");
      if (tennisOddsService == null) throw new ArgumentNullException("tennisOddsService");

      this.tennisFixtureService = tennisFixtureService;
      this.tennisPredictionService = tennisPredictionService;
      this.tennisOddsService = tennisOddsService;
    }

    public IEnumerable<TennisFixtureViewModel> GetDaysSchedule(DateTime fixtureDate)
    {
      ProgressReporterProvider.Current.ReportProgress(string.Format("Getting Days Tennis Schedule for {0}", fixtureDate.ToShortDateString()), ReporterImportance.High, ReporterAudience.Admin);

      var ret = new List<TennisFixtureViewModel>();
      var groupedCoupons = new Dictionary<string, List<TennisCouponViewModel>>();

      var tennisFixtures = new List<TennisFixtureViewModel>();
      tennisFixtures.AddRange(this.tennisPredictionService.GetTennisPredictions(fixtureDate));
      
      var tennisOdds = 
        this.tennisOddsService
            .GetAllTennisOdds(fixtureDate, tennisFixtures);

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

    public IEnumerable<TennisFixtureViewModel> UpdateDaysSchedule(DateTime fixtureDate)
    {
      ProgressReporterProvider.Current.ReportProgress(string.Format("Updating Days Tennis Schedule for {0}", fixtureDate.ToShortDateString()), ReporterImportance.High, ReporterAudience.Admin);

      var ret = new List<TennisFixtureViewModel>();

      var tennisFixturesAndPredictions = UpdateDaysFixturesAndPredicitons(fixtureDate);
      var tennisOdds = UpdateDaysOdds(fixtureDate);

      foreach (var tennisFixture in tennisFixturesAndPredictions)
      {
        TennisCouponViewModel oddsDecider;

        oddsDecider = tennisOdds.ContainsKey(tennisFixture.MatchIdentifier) ? tennisOdds[tennisFixture.MatchIdentifier] : null;
        var tennisFixtureViewModel = TennisFixtureViewModel.CreateCombination(tennisFixture, oddsDecider);
        ret.Add(tennisFixtureViewModel);
      }

      return ret;
    }

    public IEnumerable<TournamentEventViewModel> GetTournamentEvents()
    {
      ProgressReporterProvider.Current.ReportProgress("Getting Tournament Events From Tennis Betting 365", ReporterImportance.High, ReporterAudience.Admin);

      return this.tennisFixtureService.GetTournamentEvents();
    }

    public void AddTournamentCouponURL(TournamentCouponURLViewModel viewModel)
    {
      ProgressReporterProvider.Current.ReportProgress(string.Format("Adding tournament coupon for {0}", viewModel.Tournament), ReporterImportance.High, ReporterAudience.Admin);

      this.tennisOddsService.AddTournamentCouponURL(viewModel);
    }

    public IEnumerable<ShowTournamentLadderChallengeViewModel> CalculateTournamentLadderChallenge(CalculateTournamentLadderChallengeViewModel viewModel)
    {
      ProgressReporterProvider.Current.ReportProgress(string.Format("Calculating tournament ladder challenge for {0}-{1}", viewModel.Tournament, viewModel.StartDate.ToShortDateString()), ReporterImportance.High, ReporterAudience.Admin);

      var ladders = this.tennisFixtureService.GetTournamentLadder(viewModel.StartDate, viewModel.Tournament);
      if (ladders.Count() == 0)
        return Enumerable.Empty<ShowTournamentLadderChallengeViewModel>();

      var tournament = this.tennisFixtureService.GetTournament(viewModel.Tournament);

      var seed = new List<ShowTournamentLadderChallengeViewModel>();

      foreach (var ladder in ladders)
      {
        var openingRound = new ShowTournamentLadderChallengeViewModel
        {
          ExpectedWinner = string.Format("{0}, {1}", ladder.PlayerSurname, ladder.PlayerFirstName),
          ExpectedWinnerFirstName = ladder.PlayerFirstName,
          ExpectedWinnerSurname = ladder.PlayerSurname,
          RoundNumber = 0
        };
        seed.Add(openingRound);
      }

      return
        CalculateRoundTournamentLadderChallenge(seed, tournament.Slug, viewModel.StartDate.AddDays(3).Year, 1)
          .Where(x => x.RoundNumber > 0);
    }

    private IList<ShowTournamentLadderChallengeViewModel> CalculateRoundTournamentLadderChallenge(IList<ShowTournamentLadderChallengeViewModel> seed,
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
          var prediction = this.tennisPredictionService.GetSingleTennisPrediction(
            previousRoundMatches[i].ExpectedWinnerSurname,
            previousRoundMatches[i].ExpectedWinnerFirstName,
            previousRoundMatches[i + 1].ExpectedWinnerSurname,
            previousRoundMatches[i + 1].ExpectedWinnerFirstName,
            year,
            tournamentSlug,
            false /*we don't want to accidentally introduce hindsight*/);

          if (prediction.Predictions.PlayerAProbability.OutcomeProbability > prediction.Predictions.PlayerBProbability.OutcomeProbability)
            playerAGoesThrough = true;
          else if (prediction.Predictions.PlayerAProbability.OutcomeProbability < prediction.Predictions.PlayerBProbability.OutcomeProbability)
            playerAGoesThrough = false;
          else
            playerAGoesThrough = prediction.Predictions.PlayerAGames >= prediction.Predictions.PlayerBGames;

          shift = playerAGoesThrough ? i : (i + 1);
          loserShift = playerAGoesThrough ? (i + 1) : i;
          viewModel.Probability = playerAGoesThrough ? prediction.Predictions.PlayerAProbability.OutcomeProbability : prediction.Predictions.PlayerBProbability.OutcomeProbability;
        }
        viewModel.ExpectedWinner = previousRoundMatches[shift].ExpectedWinner;
        viewModel.ExpectedWinnerFirstName = previousRoundMatches[shift].ExpectedWinnerFirstName;
        viewModel.ExpectedWinnerSurname = previousRoundMatches[shift].ExpectedWinnerSurname;
        viewModel.ExpectedLoser = previousRoundMatches[loserShift].ExpectedWinner;
        viewModel.ExpectedLoserFirstName = previousRoundMatches[loserShift].ExpectedLoserFirstName;
        viewModel.ExpectedLoserSurname = previousRoundMatches[loserShift].ExpectedLoserSurname;

        seed.Add(viewModel);
      }


      return CalculateRoundTournamentLadderChallenge(seed, tournamentSlug, year, round + 1);
    }

    private IEnumerable<TennisFixtureViewModel> UpdateDaysFixturesAndPredicitons(DateTime fixtureDate)
    {
      var tennisFixtures = new List<TennisFixtureViewModel>();
      var daysMatchCount = this.tennisPredictionService.GetCountOfDaysPredictions(fixtureDate, "Tennis");
      if (daysMatchCount == 0)
        tennisFixtures.AddRange(this.tennisPredictionService.FetchTennisPredictions(fixtureDate));
      else
      {
        var daysPredictionCoupons = this.tennisPredictionService.FetchTennisPredictionCoupons(fixtureDate);
        if (daysMatchCount == daysPredictionCoupons.Count())
        {
          tennisFixtures.AddRange(this.tennisPredictionService.GetTennisPredictions(fixtureDate));
        }
        else
        {
          tennisFixtures.AddRange(this.tennisPredictionService.FetchTennisPredictions(fixtureDate));
        }
      }

      if (tennisFixtures.Count == 0)
        return Enumerable.Empty<TennisFixtureViewModel>();

      return tennisFixtures;
    }

    private Dictionary<string, TennisCouponViewModel> UpdateDaysOdds(DateTime matchDate)
    {
      var groupedCoupons = new Dictionary<string, List<TennisCouponViewModel>>();

      var daysCoupons =
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

    public IEnumerable<TennisLadderViewModel> GetTournamentLadder(DateTime matchDate, string tournament)
    {
      return this.tennisFixtureService.GetTournamentLadder(matchDate, tournament);
    }

    public void AddAlias(string source, string playerName, string valueSamuraiName, string valueSamuraiFirstName)
    {
      this.tennisFixtureService.AddAlias(source, playerName, valueSamuraiName, valueSamuraiFirstName);
    }
  }
}
