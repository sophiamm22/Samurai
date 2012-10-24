using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Model = Samurai.Domain.Model;
using Samurai.Domain.Entities;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.APIModel;

namespace Samurai.Domain.Value
{
  public abstract class AbstractPredictionStrategy
  {
    protected readonly IPredictionRepository predictionRepository;
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly IWebRepository webRepository;

    public AbstractPredictionStrategy(IPredictionRepository predictionRepository, IFixtureRepository fixtureRepository,
      IWebRepository webRepository)
    {
      this.predictionRepository = predictionRepository;
      this.fixtureRepository = fixtureRepository;
      this.webRepository = webRepository;
    }
    public abstract IEnumerable<Model.IGenericPrediction> GetPredictions(Model.IValueOptions valueOptions);
  }

  public class TennisPredictionStrategy : AbstractPredictionStrategy
  {
    public TennisPredictionStrategy(IPredictionRepository predictionRepository, IFixtureRepository fixtureRepository, 
      IWebRepository webRepository)
      : base(predictionRepository, fixtureRepository, webRepository)
    {
    }

    public override IEnumerable<Model.IGenericPrediction> GetPredictions(Model.IValueOptions valueOptions)
    {
      var predictions = new List<Model.IGenericPrediction>();

      var jsonTennisMatches = this.webRepository.GetJsonObjects<APITennisMatch>(this.predictionRepository.GetTodaysMatchesURL(),
        s => Console.WriteLine(s), string.Format("{0}-{1}", valueOptions.Competition.CompetitionName, valueOptions.CouponDate.ToShortDateString()));

      foreach (var jsonTennisMatch in jsonTennisMatches)
      {
        var jsonTennisPrediction = (APITennisPrediction)this.webRepository.ParseJson<APITennisPrediction>(
          new Uri(jsonTennisMatch.ToString()), s => Console.WriteLine(s));
        jsonTennisPrediction.StartTime = jsonTennisMatch.MatchDate;

        predictions.Add(ConvertAPIToGeneric(jsonTennisPrediction));
      }

      return predictions;
    }

    public static Model.IGenericPrediction ConvertAPIToGeneric(APITennisPrediction apiPrediction)
    {
      var tennisPrediction = new Model.TennisPrediction()
      {
        CompetitionName = apiPrediction.TournamentName,
        TeamOrPlayerA = string.Format("{0}, {1}", apiPrediction.PlayerASurname, apiPrediction.PlayerAFirstname),
        TeamOrPlayerB = string.Format("{0}, {1}", apiPrediction.PlayerBSurname, apiPrediction.PlayerBFirstname),

        PlayerAFirstName = apiPrediction.PlayerAFirstname,
        PlayerASurname = apiPrediction.PlayerASurname,
        PlayerBFirstName = apiPrediction.PlayerBFirstname,
        PlayerBSurname = apiPrediction.PlayerBSurname,

        PlayerAGames = apiPrediction.PlayerAGames,
        PlayerBGames = apiPrediction.PlayerBGames,
        MatchDate = apiPrediction.StartTime,
        Identifier = string.Format("{0} vs. {1} @ {2} on {3}", apiPrediction.PlayerASurname, apiPrediction.PlayerBSurname,
          apiPrediction.TournamentName, apiPrediction.StartTime.ToShortDateString()),
        EPoints = apiPrediction.ExpectedPoints,
        EGames = apiPrediction.ExpectedGames,
        ESets = apiPrediction.ExpectedSets
      };

      tennisPrediction.OutcomeProbabilities.Add(Model.Outcome.TeamOrPlayerA, apiPrediction.PlayerAProbability);
      tennisPrediction.OutcomeProbabilities.Add(Model.Outcome.TeamOrPlayerB, apiPrediction.PlayerBProbability);

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

  public class FootballFinkTankPredictionStrategy : AbstractPredictionStrategy
  {
    public FootballFinkTankPredictionStrategy(IPredictionRepository predictionRepository, 
      IFixtureRepository fixtureRepository, IWebRepository webRepository)
      : base(predictionRepository, fixtureRepository, webRepository)
    {
    }

    public override IEnumerable<Model.IGenericPrediction> GetPredictions(Model.IValueOptions valueOptions)
    {
      var gameWeek = this.fixtureRepository.GetDaysFootballMatches(valueOptions.Competition.CompetitionName, valueOptions.CouponDate);
      var footballTeams = new List<TeamsPlayer>();
      var predictions = new List<Model.IGenericPrediction>();

      foreach (var game in gameWeek)
      {
        var homeTeam = game.TeamsPlayerB;
        var awayTeam = game.TeamsPlayerA;

        footballTeams.Add(homeTeam);
        footballTeams.Add(awayTeam);
      }

      for (int i = 0; i < footballTeams.Count(); i += 2)
      {
        var homeTeamID = footballTeams[i].FinkTankID ?? 0;
        var awayTeamID = footballTeams[i + 1].FinkTankID ?? 0;
        var jsonFootballPredicton = (APIFootballPrediction)this.webRepository.ParseJson<APIFootballPrediction>(
          this.predictionRepository.GetFootballAPIURL(homeTeamID, awayTeamID), s => Console.WriteLine(s), string.Format("{0}-{1}", 
          valueOptions.Competition.CompetitionName, valueOptions.CouponDate.ToShortDateString()));
        predictions.Add(ConvertAPIToGeneric(jsonFootballPredicton, valueOptions.Competition, valueOptions.CouponDate));
      }
      return predictions;
    }

    private Model.IGenericPrediction ConvertAPIToGeneric(APIFootballPrediction apiPrediction, Competition competition, DateTime date)
    {
      var footballPrediction = new Model.FootballPrediction()
      {
        CompetitionName = competition.ToString(),
        TeamOrPlayerA = apiPrediction.HomeTeam,
        TeamOrPlayerB = apiPrediction.AwayTeam,
        MatchDate = date,
        Identifier = string.Format("{0} vs. {1} @ {2} on {3}", apiPrediction.HomeTeam, apiPrediction.AwayTeam,
          competition.ToString(), date.ToShortDateString())
      };

      footballPrediction.OutcomeProbabilities.Add(Model.Outcome.TeamOrPlayerA, apiPrediction.ExpectedProbabilities.HomeWinProb);
      footballPrediction.OutcomeProbabilities.Add(Model.Outcome.Draw, apiPrediction.ExpectedProbabilities.DrawProb);
      footballPrediction.OutcomeProbabilities.Add(Model.Outcome.TeamOrPlayerB, apiPrediction.ExpectedProbabilities.AwayWinProb);

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
