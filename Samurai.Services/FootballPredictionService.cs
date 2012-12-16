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
  public abstract class PredictionService
  {
    protected readonly IPredictionStrategyProvider predictionProvider;
    protected readonly IPredictionRepository predictionRepository;
    protected readonly IFixtureRepository fixtureRepository;

    public PredictionService(IPredictionStrategyProvider predictionProvider,
      IPredictionRepository predictionRepository, IFixtureRepository fixtureRepository)
    {
      if (predictionProvider == null) throw new ArgumentNullException("predictionProvider");
      if (predictionRepository == null) throw new ArgumentNullException("predictionRepository");
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");

      this.predictionProvider = predictionProvider;
      this.predictionRepository = predictionRepository;
      this.fixtureRepository = fixtureRepository;
    }

    protected IEnumerable<Match> PersistGenericPredictions(IEnumerable<GenericPrediction> predictions)
    {
      var matches = new List<Match>();

      foreach (var prediction in predictions)
      {
        var teamA = this.fixtureRepository.GetTeamOrPlayerFromNameAndMaybeFirstName(prediction.TeamOrPlayerA, prediction.PlayerAFirstName);
        var teamB = this.fixtureRepository.GetTeamOrPlayerFromNameAndMaybeFirstName(prediction.TeamOrPlayerB, prediction.PlayerBFirstName);

        var match = this.fixtureRepository.GetMatchFromTeamSelections(teamA, teamB, prediction.MatchDate);
        if (match == null)
        {
          var tournamentEvent = this.fixtureRepository.GetTournamentEventFromTournamentAndDate(prediction.MatchDate, prediction.TournamentName);
          match = this.fixtureRepository.CreateMatch(teamA, teamB, prediction.MatchDate, tournamentEvent);
        }
        matches.Add(match);

        foreach (var outcome in prediction.OutcomeProbabilities)
        {
          var persistedOutcome = match.MatchOutcomeProbabilitiesInMatches
                                      .FirstOrDefault(o => o.MatchOutcome.Id == (int)outcome.Key);

          if (persistedOutcome == null)
          {
            match.MatchOutcomeProbabilitiesInMatches.Add(new MatchOutcomeProbabilitiesInMatch
            {
              Match = match,
              MatchOutcome = this.fixtureRepository.GetMatchOutcomeByID((int)outcome.Key),
              MatchOutcomeProbability = (decimal)outcome.Value
            });
          }
          else
          {
            persistedOutcome.MatchOutcomeProbability = (decimal)outcome.Value;
          }
        }

        foreach (var scoreLine in prediction.ScoreLineProbabilities)
        {
          var persistedScoreLine = match.ScoreOutcomeProbabilitiesInMatches
                                        .FirstOrDefault(s => string.Format("{0}-{1}", s.ScoreOutcome.TeamAScore, s.ScoreOutcome.TeamBScore) == scoreLine.Key);
          if (persistedScoreLine == null)
          {
            if (scoreLine.Value.HasValue)
            {
              match.ScoreOutcomeProbabilitiesInMatches.Add(new ScoreOutcomeProbabilitiesInMatch
              {
                Match = match,
                ScoreOutcome = this.fixtureRepository.GetScoreOutcome(int.Parse(scoreLine.Key.Split('-')[0]), int.Parse(scoreLine.Key.Split('-')[1])),
                ScoreOutcomeProbability = (decimal)(scoreLine.Value ?? 0.0)
              });
            }
          }
          else
          {
            persistedScoreLine.ScoreOutcomeProbability = (decimal)(scoreLine.Value ?? 0.0); //should be nullable
          }
        }
      }
      this.fixtureRepository.SaveChanges();
      return matches;
    }
  }

  public class FootballPredictionService : PredictionService, IFootballPredictionService
  {
    public FootballPredictionService(IPredictionStrategyProvider predictionProvider,
      IPredictionRepository predictionRepository, IFixtureRepository fixtureRepository)
      : base(predictionProvider, predictionRepository, fixtureRepository)
    { }

    public IEnumerable<FootballFixtureViewModel> GetFootballPredictions(IEnumerable<FootballFixtureViewModel> fixtures)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<FootballFixtureViewModel> FetchFootballPredictions(IEnumerable<FootballFixtureViewModel> fixtures)
    {
      var predictions = GetGenericFootballPredictionsFromViewModelFixtures(fixtures);
      var matches = PersistGenericPredictions(predictions);
      return Mapper.Map<IEnumerable<Match>, IEnumerable<FootballFixtureViewModel>>(matches);
    }

    private IEnumerable<FootballPrediction> GetGenericFootballPredictionsFromViewModelFixtures(IEnumerable<FootballFixtureViewModel> fixtures)
    {
      var predictions = new List<FootballPrediction>();
      var source = this.fixtureRepository.GetExternalSource("Fink Tank (dectech)");
      var football = this.fixtureRepository.GetSport("Football");
      var predictionStrategy = this.predictionProvider.CreatePredictionStrategy(football);

      (from fixture in fixtures
       group fixture by fixture.League into byLeagues
       let tournament = this.fixtureRepository.GetTournament(fixtures.First(f => f.League == byLeagues.Key).League)
       select new
       {
         LeagueGroup = byLeagues.Key,
         DateGroups = (from fixture in byLeagues
                       group fixture by fixture.MatchDate.Date into byDates
                       select new
                       {
                         Date = byDates.Key,
                         ValueOptions = new ValueOptions
                         {
                           Tournament = tournament,
                           Sport = football,
                           CouponDate = byDates.Key,
                           OddsSource = source
                         },
                         Fixtures = byDates.ToList()
                       })
                       .ToList()
       })
      .ToList()
      .ForEach(p =>
        predictions.AddRange(p.DateGroups
                              .SelectMany(f => predictionStrategy.GetPredictions(f.ValueOptions))
                              .Cast<FootballPrediction>()));

      return predictions;
    }
  }
}
