using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model = Samurai.Domain.Model;
using Samurai.Domain.Entities;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.APIModel;

namespace Samurai.Domain.Value.Async
{
  public class TennisAsyncPredictionStrategy : AbstractAsyncPredictionStrategy, IAsyncPredictionStrategy
  {
    public TennisAsyncPredictionStrategy(IPredictionRepository predictionRepository,
      IFixtureRepository fixtureRepository, IWebRepositoryProviderAsync webRepositoryProvider)
      : base(predictionRepository, fixtureRepository, webRepositoryProvider)
    {
    }

    public async Task<IEnumerable<Model.GenericPrediction>> FetchPredictionsAsync(Model.IValueOptions valueOptions)
    {
      var predictions = new List<Model.GenericPrediction>();
      var webRepository =
        this.webRepositoryProvider
            .CreateWebRepository(valueOptions.CouponDate);

      var jsonTennisMatches =
        await webRepository.ParseJsonEnumerable<APITennisMatch>(
          this.predictionRepository.GetTodaysMatchesURL(),
          string.Format("atp-{0}", valueOptions.CouponDate.ToShortDateString()));

      foreach (var jsonTennisMatch in jsonTennisMatches)
      {
        var predictionURL = new Uri(jsonTennisMatch.ToString());

        var jsonTennisPrediction = await
          webRepository.ParseJson<APITennisPrediction>(predictionURL);

        jsonTennisPrediction.StartTime = jsonTennisMatch.MatchDate;

        predictions.Add(ConvertAPIToGeneric(jsonTennisPrediction, predictionURL));
      }

      return predictions;
    }

    public async Task<IEnumerable<Model.GenericPrediction>> FetchPredictionsCouponAsync(Model.IValueOptions valueOptions)
    {
      var predictions = new List<Model.GenericPrediction>();
      var webRepository =
        this.webRepositoryProvider
            .CreateWebRepository(valueOptions.CouponDate);

      var jsonTennisMatches =
        await webRepository.ParseJsonEnumerable<APITennisMatch>(
          this.predictionRepository.GetTodaysMatchesURL(),
          string.Format("atp-{0}", valueOptions.CouponDate.ToShortDateString()));

      foreach (var jsonTennisMatch in jsonTennisMatches)
      {
        predictions.Add(new Model.TennisPrediction()
        {
          PlayerAFirstName = jsonTennisMatch.PlayerAFirstName,
          TeamOrPlayerA = jsonTennisMatch.PlayerASurname,
          PlayerBFirstName = jsonTennisMatch.PlayerBSurname,
          TeamOrPlayerB = jsonTennisMatch.PlayerBSurname,
        });
      }
      return predictions;
    }

    public async Task<Model.GenericPrediction> FetchSinglePredictionAsync(TeamPlayer teamPlayerA, TeamPlayer teamPlayerB, Tournament tournament, Model.IValueOptions valueOptions)
    {
      var webRepository =
        this.webRepositoryProvider
            .CreateWebRepository(valueOptions.CouponDate);

      var predictionURL =
        this.predictionRepository
            .GetTennisPredictionURL(teamPlayerA, teamPlayerB, tournament, valueOptions.CouponDate);

      var jsonTennisPrediction = await
        webRepository.ParseJson<APITennisPrediction>(predictionURL);

      jsonTennisPrediction.StartTime = valueOptions.CouponDate;

      return ConvertAPIToGeneric(jsonTennisPrediction, predictionURL);
    }

    public static Model.GenericPrediction ConvertAPIToGeneric(APITennisPrediction apiPrediction, Uri predictionURL)
    {
      var tennisPrediction = new Model.TennisPrediction()
      {
        TournamentName = apiPrediction.TournamentName,
        PredictionURL = predictionURL,

        PlayerAFirstName = apiPrediction.PlayerAFirstname,
        TeamOrPlayerA = apiPrediction.PlayerASurname,
        PlayerBFirstName = apiPrediction.PlayerBFirstname,
        TeamOrPlayerB = apiPrediction.PlayerBSurname,

        PlayerAGames = apiPrediction.PlayerAGames,
        PlayerBGames = apiPrediction.PlayerBGames,
        MatchDate = apiPrediction.StartTime,
        MatchIdentifier = string.Format("{0} vs. {1} @ {2} on {3}", apiPrediction.PlayerASurname, apiPrediction.PlayerBSurname,
          apiPrediction.TournamentName, apiPrediction.StartTime.ToShortDateString()),
        EPoints = apiPrediction.ExpectedPoints,
        EGames = apiPrediction.ExpectedGames,
        ESets = apiPrediction.ExpectedSets
      };

      tennisPrediction.OutcomeProbabilities.Add(Model.Outcome.HomeWin, apiPrediction.PlayerAProbability);
      tennisPrediction.OutcomeProbabilities.Add(Model.Outcome.AwayWin, apiPrediction.PlayerBProbability);

      if (apiPrediction.FiveSets)
      {
        tennisPrediction.ScoreLineProbabilities.Add("3-0", apiPrediction.ProbThreeLove);
        tennisPrediction.ScoreLineProbabilities.Add("3-1", apiPrediction.ProbThreeOne);
        tennisPrediction.ScoreLineProbabilities.Add("3-2", apiPrediction.ProbThreeTwo);
        tennisPrediction.ScoreLineProbabilities.Add("2-3", apiPrediction.ProbTwoThree);
        tennisPrediction.ScoreLineProbabilities.Add("1-3", apiPrediction.ProbOneThree);
        tennisPrediction.ScoreLineProbabilities.Add("0-3", apiPrediction.ProbLoveThree);
      }
      else
      {
        tennisPrediction.ScoreLineProbabilities.Add("2-0", apiPrediction.ProbTwoLove);
        tennisPrediction.ScoreLineProbabilities.Add("2-1", apiPrediction.ProbTwoOne);
        tennisPrediction.ScoreLineProbabilities.Add("1-2", apiPrediction.ProbOneTwo);
        tennisPrediction.ScoreLineProbabilities.Add("0-2", apiPrediction.ProbLoveTwo);
      }
      return tennisPrediction;
    }
  }
}
