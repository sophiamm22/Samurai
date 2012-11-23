using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Entities;
using Samurai.Domain.Value;
using Samurai.Domain.Model;


namespace Samurai.Services
{
  public class TennisPredictionService : PredictionService, ITennisPredictionService
  {
    public TennisPredictionService(IPredictionStrategyProvider predictionProvider,
      IPredictionRepository predictionRepository, IFixtureRepository fixtureRepository)
      : base(predictionProvider, predictionRepository, fixtureRepository)
    { }

    public IEnumerable<TennisMatchViewModel> GetTennisPredictions(DateTime matchDate)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<TennisMatchViewModel> FetchTennisPredictions(DateTime matchDate)
    {
      var predictions = GetModelTennisPredictions(matchDate);
      var matches = PersistTennisPredictions(predictions);
      return Mapper.Map<IEnumerable<Match>, IEnumerable<TennisMatchViewModel>>(matches);
    }

    private IEnumerable<TennisPrediction> GetModelTennisPredictions(DateTime matchDate)
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
      var tennisPredictions = predictionStrategy.GetPredictions(valueOptions)
                                                .Cast<TennisPrediction>()
                                                .ToList();
      return tennisPredictions;
    }

    private IEnumerable<Match> PersistTennisPredictions(IEnumerable<TennisPrediction> tennisPredictions)
    {
      var persistedMatches = PersistGenericPredictions(tennisPredictions);

      persistedMatches.Select(p => p.Id)
                      .Zip(tennisPredictions, (m, p) => new
                        {
                          MatchID = m,
                          Prediction = p
                        })
                      .ToList()
                      .ForEach(m =>
                        {
                          var prediction = new TennisPredictionStat
                          {
                            PlayerAGames = m.Prediction.PlayerAGames,
                            PlayerBGames = m.Prediction.PlayerBGames,
                            EPoints = (decimal?)m.Prediction.EPoints,
                            EGames = (decimal?)m.Prediction.EGames,
                            ESets = (decimal?)m.Prediction.ESets
                          };
                          this.predictionRepository
                              .AddOrUpdateTennisPredictionsStats(m.MatchID,
                                                                 prediction);
                        });

      return persistedMatches;
    }

  }
}
