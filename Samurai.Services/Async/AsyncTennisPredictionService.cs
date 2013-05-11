using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Services.Contracts.Async;
using Samurai.Domain.Value.Async;
using Samurai.Web.ViewModels.Tennis;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Domain.Model;

namespace Samurai.Services.Async
{
  public class AsyncTennisPredictionService : AsyncPredictionService, IAsyncTennisPredictionService
  {
    public AsyncTennisPredictionService(IAsyncPredictionStrategyProvider predictionProvider,
      IPredictionRepository predictionRepository, IFixtureRepository fixtureRepository, IStoredProceduresRepository storedProcRepository)
      : base(predictionProvider, predictionRepository, fixtureRepository, storedProcRepository)
    { }

    public async Task<IEnumerable<TennisFixtureViewModel>> GetTennisPredictions(DateTime matchDate)
    {
      return await Task.Run(() => GetTennisPredictionsSync(matchDate));
    }

    public async Task<IEnumerable<TennisFixtureViewModel>> FetchTennisPredictions(DateTime matchDate)
    {
      var predictions = await FetchGenericTennisPredictions(matchDate);
      var combinedStats = await PersistTennisPredictions(predictions, matchDate);

      return Mapper.Map<IEnumerable<TennisMatchDetail>, IEnumerable<TennisFixtureViewModel>>(combinedStats);
    }

    public async Task<IEnumerable<TennisFixtureViewModel>> FetchTennisPredictionCoupons(DateTime matchDate)
    {
      var predictions = await FetchGenericTennisPredictionCoupons(matchDate);
      var predictionsWrapped =
        predictions.Select(x => new TennisMatchDetail
        {
          TennisPrediction = x
        });

      return Mapper.Map<IEnumerable<TennisMatchDetail>, IEnumerable<TennisFixtureViewModel>>(predictionsWrapped);
    }

    public async Task<TennisFixtureViewModel> GetSingleTennisPrediction(string playerASurname, string playerAFirstname, string playerBSurname, string playerBFirstname, int year, string tournamentSlug, bool updateStats = true)
    {
      var sport = this.fixtureRepository.GetSport("Tennis");
      var tournament = this.fixtureRepository.GetTournamentFromSlug(tournamentSlug);
      var playerA = this.fixtureRepository.GetTeamOrPlayerFromNameAndMaybeFirstName(playerASurname, playerAFirstname);
      var playerB = this.fixtureRepository.GetTeamOrPlayerFromNameAndMaybeFirstName(playerBSurname, playerBFirstname);
      var safeDate = new DateTime(year, 06, 29);
      var valueOptions = new ValueOptions
      {
        CouponDate = safeDate,
        Tournament = tournament,
        DontUpdateTennisStats = !updateStats
      };

      var provider = this.predictionProvider.CreatePredictionStrategy(sport);
      var prediction = 
        (TennisPrediction)await provider.FetchSinglePredictionAsync(playerA, playerB, tournament, valueOptions);

      var tennisPredictionStat = new TennisPredictionStat
      {
        PlayerAGames = prediction.PlayerAGames,
        PlayerBGames = prediction.PlayerBGames,
        EPoints = (decimal?)prediction.EPoints,
        EGames = (decimal?)prediction.EGames,
        ESets = (decimal?)prediction.ESets
      };
      var tennisMatchDetail = new TennisMatchDetail()
      {
        TennisPredictionStat = tennisPredictionStat,
        TennisPrediction = prediction
      };

      return Mapper.Map<TennisMatchDetail, TennisFixtureViewModel>(tennisMatchDetail); //no idea what HydrateFullTennisMatchDetails was for in the first place, I only need predictions and number of games for now
    
    }

    private IEnumerable<TennisFixtureViewModel> GetTennisPredictionsSync(DateTime matchDate)
    {
      var tennisPredictionsDic = new Dictionary<int, TennisPrediction>();
      var matches =
        this.fixtureRepository
            .GetDaysMatches(matchDate, "Tennis")
            .ToList();

      var tennisPredictionStatsDic =
        this.predictionRepository
            .GetTennisPredictionStatByMatchIDs(matches.Select(m => m.Id))
            .ToDictionary(x => x.Id);

      foreach (var match in matches)
      {
        var playerA = this.fixtureRepository.GetTeamOrPlayerById(match.TeamAID);
        var playerB = this.fixtureRepository.GetTeamOrPlayerById(match.TeamBID);
        var tournamentEvent = this.fixtureRepository.GetTournamentEventById(match.TournamentEventID);
        var tournament = this.fixtureRepository.GetTournamentFromTournamentEvent(tournamentEvent.EventName);

        var identifier = string.Format("{0},{1}/vs/{2},{3}/{4}/{5}", playerA.Name, playerA.FirstName, playerB.Name, playerB.FirstName,
          tournamentEvent.EventName, matchDate.Date.ToShortDateString().Replace("/", "-"));

        var tennisPrediction = new TennisPrediction
        {
          MatchID = match.Id,
          MatchIdentifier = identifier,
          TournamentName = tournament.TournamentName,
          MatchDate = match.MatchDate,
          TeamOrPlayerA = playerA.Name,
          PlayerAFirstName = playerA.FirstName,
          TeamOrPlayerB = playerB.Name,
          PlayerBFirstName = playerB.FirstName,
        };

        tennisPredictionsDic.Add(match.Id, tennisPrediction);
      }

      var outcomePredictions = this.predictionRepository.GetMatchOutcomeProbabilitiesInMatchByIDs(matches.Select(x => x.Id));
      var scoreLinePredictions = this.predictionRepository.GetScoreOutcomeProbabilitiesInMatchByIDs(matches.Select(x => x.Id));

      foreach (var id in matches.Select(x => x.Id))
      {
        var tennisPrediction = tennisPredictionsDic[id];

        tennisPrediction.OutcomeProbabilities = outcomePredictions[id].ToDictionary(o => (Outcome)o.MatchOutcomeID, o => (double)o.MatchOutcomeProbability);
        if (scoreLinePredictions.ContainsKey(id))
          tennisPrediction.ScoreLineProbabilities = scoreLinePredictions[id].ToDictionary(o => string.Format("{0}-{1}", o.ScoreOutcome.TeamAScore, o.ScoreOutcome.TeamBScore), o => (double?)o.ScoreOutcomeProbability);
      }

      var combinedStats = HydrateFullTennisMatchDetails(matchDate, tennisPredictionsDic,
        tennisPredictionStatsDic);

      return Mapper.Map<IEnumerable<TennisMatchDetail>, IEnumerable<TennisFixtureViewModel>>(combinedStats);
    }

    private async Task<IEnumerable<TennisPrediction>> FetchGenericTennisPredictionCoupons(DateTime matchDate)
    {
      var sport = this.fixtureRepository.GetSport("Tennis");
      var tournament = this.fixtureRepository.GetTournament("ATP");
      var couponDate = matchDate.Date;
      var source = this.fixtureRepository.GetExternalSource("Tennis Betting 365");

      var valueOptions = new ValueOptions
      {
        Tournament = tournament,
        Sport = sport,
        CouponDate = matchDate,
        OddsSource = source
      };

      var predictionStrategy = 
        this.predictionProvider
            .CreatePredictionStrategy(sport);

      var predictions = await
        predictionStrategy.FetchPredictionsCouponAsync(valueOptions);

      var tennisPredictions = 
        predictions.Cast<TennisPrediction>()
                   .ToList();

      return tennisPredictions;
    }

    private async Task<IEnumerable<TennisPrediction>> FetchGenericTennisPredictions(DateTime matchDate)
    {
      var sport = this.fixtureRepository.GetSport("Tennis");
      var tournament = this.fixtureRepository.GetTournament("ATP");
      var couponDate = matchDate.Date;
      var source = this.fixtureRepository.GetExternalSource("Tennis Betting 365");

      var valueOptions = new ValueOptions
      {
        Tournament = tournament,
        Sport = sport,
        CouponDate = matchDate,
        OddsSource = source
      };

      var predictionStrategy = 
        this.predictionProvider
            .CreatePredictionStrategy(sport);

      var predictions = await 
        predictionStrategy.FetchPredictionsAsync(valueOptions);

      var tennisPredictions = 
        predictions.Cast<TennisPrediction>()
                   .ToList();

      return tennisPredictions;
    }

    private async Task<IEnumerable<TennisMatchDetail>> PersistTennisPredictions(IEnumerable<TennisPrediction> tennisPredictions, DateTime matchDate)
    {
      var persistedMatchIds = await PersistGenericPredictions(tennisPredictions);
      var tennisPredictionsDic = new Dictionary<int, TennisPrediction>();
      var tennisPredictionStatsDic = new Dictionary<int, TennisPredictionStat>();
      var ret = new List<TennisMatchDetail>();

      persistedMatchIds.Zip(tennisPredictions, (m, p) => new
      {
        MatchID = m,
        Prediction = p
      })
      .ToList()
      .ForEach(m =>
      {
        var predictionStat = new TennisPredictionStat
        {
          Id = m.MatchID,
          PlayerAGames = m.Prediction.PlayerAGames,
          PlayerBGames = m.Prediction.PlayerBGames,
          EPoints = (decimal?)m.Prediction.EPoints,
          EGames = (decimal?)m.Prediction.EGames,
          ESets = (decimal?)m.Prediction.ESets
        };
        this.predictionRepository
            .AddOrUpdateTennisPredictionsStats(predictionStat);

        tennisPredictionStatsDic.Add(m.MatchID, predictionStat);
        tennisPredictionsDic.Add(m.MatchID, m.Prediction);
      });

      await Task.Run(() => this.predictionRepository.SaveChanges());

      return HydrateFullTennisMatchDetails(matchDate, tennisPredictionsDic, tennisPredictionStatsDic);
    }

    private IEnumerable<TennisMatchDetail> HydrateFullTennisMatchDetails(DateTime matchDate,
      Dictionary<int, TennisPrediction> tennisPredictionsDic, Dictionary<int, TennisPredictionStat> tennisPredictionStatsDic)
    {
      var ret = new List<TennisMatchDetail>();

      var genericMatchDetailsDic = this.storedProcRepository
                                 .GetGenericMatchDetails(matchDate, "Tennis")
                                 .ToDictionary(x => x.MatchID);

      foreach (var matchId in genericMatchDetailsDic.Keys)
      {
        var genericMatchDetailQuery = genericMatchDetailsDic[matchId];
        var genericMatchDetail = Mapper.Map<GenericMatchDetailQuery, GenericMatchDetail>(genericMatchDetailQuery);

        var tennisPrediction = tennisPredictionsDic[matchId];
        var tennisPredictionStat = tennisPredictionStatsDic[matchId];


        var combinedStats = Mapper.Map<GenericMatchDetail, TennisMatchDetail>(genericMatchDetail);
        combinedStats.TennisPrediction = tennisPrediction;
        combinedStats.TennisPredictionStat = tennisPredictionStat;
        Mapper.Map<TennisPredictionStat, TennisPrediction>(tennisPredictionStat, combinedStats.TennisPrediction);
        ret.Add(combinedStats);
      }

      return ret;
    }

  }
}
