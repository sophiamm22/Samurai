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
  public class FootballAsyncPredictionStrategy : AbstractAsyncPredictionStrategy, IAsyncPredictionStrategy
  {
    public FootballAsyncPredictionStrategy(IPredictionRepository predictionRepository,
      IFixtureRepository fixtureRepository, IWebRepositoryProviderAsync webRepositoryProvider)
      : base(predictionRepository, fixtureRepository, webRepositoryProvider)
    {
    }

    public async Task<IEnumerable<Model.GenericPrediction>> FetchPredictionsAsync(Model.IValueOptions valueOptions)
    {
      var daysMatches =
        this.fixtureRepository
            .GetDaysMatches(valueOptions.Tournament.TournamentName, valueOptions.CouponDate);
      var predictionURLs = new List<Uri>();
      var predictions = new List<Model.GenericPrediction>();

      var webRepository =
        this.webRepositoryProvider
            .CreateWebRepository(valueOptions.CouponDate);

      foreach (var match in daysMatches)
      {
        var predictionURL =
          this.predictionRepository
              .GetFootballAPIURL(int.Parse(match.TeamsPlayerA.ExternalID),
                                 int.Parse(match.TeamsPlayerB.ExternalID));
        predictionURLs.Add(predictionURL);
      }

      var apiModels = await webRepository.ParseJsons<APIFootballPrediction>(predictionURLs);

      return apiModels.Zip(predictionURLs, (m, u) => new { model = m, url = u })
                      .ToList()
                      .Select(x => ConvertAPIToGeneric(x.model,
                                                       valueOptions.Tournament,
                                                       valueOptions.CouponDate,
                                                       x.url));
    }

    public Task<IEnumerable<Model.GenericPrediction>> FetchPredictionsCouponAsync(Model.IValueOptions valueOptions)
    {
      throw new NotImplementedException();
    }

    public Task<Model.GenericPrediction> FetchSinglePredictionAsync(TeamPlayer teamPlayerA, TeamPlayer teamPlayerB, Tournament tournament, Model.IValueOptions valueOptions)
    {
      throw new NotImplementedException();
    }

    private Model.GenericPrediction ConvertAPIToGeneric(APIFootballPrediction apiPrediction, Tournament tournament, DateTime date, Uri predictionURL)
    {
      var tournamentEvent = this.fixtureRepository.GetTournamentEventFromTournamentAndDate(date, tournament.TournamentName);

      var footballPrediction = new Model.FootballPrediction()
      {
        TournamentName = tournament.TournamentName,
        TournamentEventName = tournamentEvent.EventName,
        TeamOrPlayerA = apiPrediction.HomeTeam,
        TeamOrPlayerB = apiPrediction.AwayTeam,
        MatchDate = date,
        MatchIdentifier = string.Format("{0}/vs/{1}/{2}/{3}", apiPrediction.HomeTeam, apiPrediction.AwayTeam,
          tournamentEvent.EventName, date.ToShortDateString().Replace("/", "-"))
      };

      footballPrediction.OutcomeProbabilities.Add(Model.Outcome.HomeWin, apiPrediction.ExpectedProbabilities.HomeWinProb);
      footballPrediction.OutcomeProbabilities.Add(Model.Outcome.Draw, apiPrediction.ExpectedProbabilities.DrawProb);
      footballPrediction.OutcomeProbabilities.Add(Model.Outcome.AwayWin, apiPrediction.ExpectedProbabilities.AwayWinProb);

      foreach (var scoreLine in apiPrediction.ScoreProbabilities)
      {
        var key = string.Format("{0}-{1}", scoreLine.HomeGoals.ToString(), scoreLine.AwayGoals.ToString());
        if (!footballPrediction.ScoreLineProbabilities.ContainsKey(key))
          footballPrediction.ScoreLineProbabilities.Add(key, scoreLine.Probability);
      }
      return footballPrediction;
    }


  }

}
