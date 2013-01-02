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

    public IEnumerable<TennisPredictionViewModel> GetTennisPredictions(DateTime matchDate)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<TennisPredictionViewModel> FetchTennisPredictions(DateTime matchDate)
    {
      var predictions = GetModelTennisPredictions(matchDate);
      PersistTennisPredictions(predictions);
      return Mapper.Map<IEnumerable<TennisPrediction>, IEnumerable<TennisPredictionViewModel>>(predictions);
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
      var tennisPredictions = predictionStrategy.FetchPredictions(valueOptions)
                                                .Cast<TennisPrediction>()
                                                .ToList();
      return tennisPredictions;
    }

    private void PersistTennisPredictions(IEnumerable<TennisPrediction> tennisPredictions)
    {
      var persistedMatches = PersistGenericPredictions(tennisPredictions);

      persistedMatches.Zip(tennisPredictions, (m, p) => new
                        {
                          MatchID = m,
                          Prediction = p
                        })
                      .ToList()
                      .ForEach(m =>
                        {
                          var prediction = new TennisPredictionStat
                          {
                            Id = m.MatchID,
                            PlayerAGames = m.Prediction.PlayerAGames,
                            PlayerBGames = m.Prediction.PlayerBGames,
                            EPoints = (decimal?)m.Prediction.EPoints,
                            EGames = (decimal?)m.Prediction.EGames,
                            ESets = (decimal?)m.Prediction.ESets
                          };
                          this.predictionRepository
                              .AddOrUpdateTennisPredictionsStats(prediction);
                        });
      this.predictionRepository.SaveChanges();
    }

  }
}
