using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels.Tennis;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Domain.Value;
using Samurai.Domain.Model;


namespace Samurai.Services
{
  public class TennisPredictionService : PredictionService, ITennisPredictionService
  {
    public TennisPredictionService(IPredictionStrategyProvider predictionProvider,
      IPredictionRepository predictionRepository, IFixtureRepository fixtureRepository, IStoredProceduresRepository storedProcRepository)
      : base(predictionProvider, predictionRepository, fixtureRepository, storedProcRepository)
    { }

    public IEnumerable<TennisFixtureViewModel> GetTennisPredictions(DateTime matchDate)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<TennisFixtureViewModel> FetchTennisPredicitonsNew(DateTime matchDate)
    {
      var predictions = FetchGenericTennisPredictions(matchDate);
      var combinedStats = PersistTennisPredictions(predictions, matchDate);

      return Mapper.Map<IEnumerable<TennisMatchDetail>, IEnumerable<TennisFixtureViewModel>>(combinedStats);
    }

    public IEnumerable<TennisFixtureViewModel> FetchTennisPredictions(DateTime matchDate)
    {
      var predictions = FetchGenericTennisPredictions(matchDate);
      PersistTennisPredictions(predictions, matchDate);
      return Mapper.Map<IEnumerable<TennisPrediction>, IEnumerable<TennisFixtureViewModel>>(predictions);
    }

    private IEnumerable<TennisPrediction> FetchGenericTennisPredictions(DateTime matchDate)
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

      var predictionStrategy = this.predictionProvider.CreatePredictionStrategy(sport);
      var tennisPredictions = predictionStrategy.FetchPredictions(valueOptions)
                                                .Cast<TennisPrediction>()
                                                .ToList();
      return tennisPredictions;
    }

    private IEnumerable<TennisMatchDetail> PersistTennisPredictions(IEnumerable<TennisPrediction> tennisPredictions, DateTime matchDate)
    {
      var persistedMatchIds = PersistGenericPredictions(tennisPredictions);
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
      this.predictionRepository.SaveChanges();

      var genericMatchDetailsDic = this.storedProcRepository
                                       .GetGenericMatchDetails(matchDate, "Tennis")
                                       .ToDictionary(x => x.MatchID);

      foreach (var matchId in persistedMatchIds)
      {
        var genericMatchDetailQuery = genericMatchDetailsDic[matchId];
        var genericMatchDetail = Mapper.Map<GenericMatchDetailQuery, GenericMatchDetail>(genericMatchDetailQuery);

        var tennisPrediction = tennisPredictionsDic[matchId];
        var tennisPredictionStat = tennisPredictionStatsDic[matchId];

        var combinedStats = Mapper.Map<GenericMatchDetail, TennisMatchDetail>(genericMatchDetail);
        combinedStats.TennisPrediction = tennisPrediction;
        combinedStats.TennisPredictionStat = tennisPredictionStat;
        ret.Add(combinedStats);
      }

      return ret;
    }

  }
}
