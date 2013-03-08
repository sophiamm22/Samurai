using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reg = System.Text.RegularExpressions;

using AutoMapper;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Entities;
using Samurai.Domain.Value;
using Samurai.Domain.Model;

namespace Samurai.Services.AdminServices
{
  public abstract class PredictionService : IPredictionAdminService
  {
    protected readonly IPredictionStrategyProvider predictionProvider;
    protected readonly IPredictionRepository predictionRepository;
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly IStoredProceduresRepository storedProcRepository;

    public PredictionService(IPredictionStrategyProvider predictionProvider,
      IPredictionRepository predictionRepository, IFixtureRepository fixtureRepository, IStoredProceduresRepository storedProcRepository)
    {
      if (predictionProvider == null) throw new ArgumentNullException("predictionProvider");
      if (predictionRepository == null) throw new ArgumentNullException("predictionRepository");
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (storedProcRepository == null) throw new ArgumentException("storedProcRepository");

      this.predictionProvider = predictionProvider;
      this.predictionRepository = predictionRepository;
      this.fixtureRepository = fixtureRepository;
      this.storedProcRepository = storedProcRepository;
    }

    public int GetCountOfDaysPredictions(DateTime fixtureDate, string sport)
    {
      var probCount = this.predictionRepository.GetMatchOutcomeProbabiltiesInMatchByDate(fixtureDate, sport)
        .Count();
      return sport == "Football" ? (probCount / 3) : (probCount / 2);
    }

    protected IEnumerable<int> PersistGenericPredictions(IEnumerable<GenericPrediction> predictions)
    {
      var matchIDs = new List<int>();

      foreach (var prediction in predictions)
      {
        var teamA = this.fixtureRepository.GetTeamOrPlayerFromNameAndMaybeFirstName(prediction.TeamOrPlayerA, prediction.PlayerAFirstName);
        var teamB = this.fixtureRepository.GetTeamOrPlayerFromNameAndMaybeFirstName(prediction.TeamOrPlayerB, prediction.PlayerBFirstName);

        var match = this.fixtureRepository.GetMatchFromTeamSelections(teamA, teamB, prediction.MatchDate);
        if (match == null)
        {
          var tournamentName = Reg.Regex.Replace(prediction.TournamentName, @" 20\d{2}", "");

          var tournamentEvent = this.fixtureRepository.GetTournamentEventFromTournamentAndDate(prediction.MatchDate, tournamentName);
          match = this.fixtureRepository.CreateMatch(teamA, teamB, prediction.MatchDate, tournamentEvent);
        }
        matchIDs.Add(match.Id);

        var persistedOutcomes = this.predictionRepository.GetMatchOutcomeProbabilities(match.Id).ToList();

        foreach (var outcome in prediction.OutcomeProbabilities)
        {
          var persistedOutcome = persistedOutcomes.FirstOrDefault(p => p.MatchOutcomeID == (int)outcome.Key); //match.MatchOutcomeProbabilitiesInMatches.FirstOrDefault(o => o.MatchOutcome.Id == (int)outcome.Key);

          if (persistedOutcome == null)
          {
            var newOutcomeProb = new MatchOutcomeProbabilitiesInMatch
            {
              MatchID = match.Id,
              MatchOutcome = this.fixtureRepository.GetMatchOutcomeByID((int)outcome.Key),
              MatchOutcomeProbability = (decimal)outcome.Value
            };
            this.predictionRepository.AddMatchOutcomeProbabilitiesInMatch(newOutcomeProb);
          }
          else
          {
            persistedOutcome.MatchOutcomeProbability = (decimal)outcome.Value;
          }
        }

        var persistedScoreLines = this.predictionRepository.GetScoreOutcomeProbabilities(match.Id).ToList();

        foreach (var scoreLine in prediction.ScoreLineProbabilities)
        {
          var persistedScoreLine = persistedScoreLines.FirstOrDefault(s => string.Format("{0}-{1}", s.ScoreOutcome.TeamAScore, s.ScoreOutcome.TeamBScore) == scoreLine.Key);

          if (persistedScoreLine == null)
          {
            if (scoreLine.Value.HasValue)
            {
              var newScoreOutcomeProbabilty = new ScoreOutcomeProbabilitiesInMatch
              {
                MatchID = match.Id,
                ScoreOutcome = this.fixtureRepository.GetScoreOutcome(int.Parse(scoreLine.Key.Split('-')[0]), int.Parse(scoreLine.Key.Split('-')[1])),
                ScoreOutcomeProbability = (decimal)(scoreLine.Value ?? 0.0)
              };
              this.predictionRepository.AddScoreOutcomeProbabilities(newScoreOutcomeProbabilty);
            }
          }
          else
          {
            persistedScoreLine.ScoreOutcomeProbability = (decimal)(scoreLine.Value ?? 0.0); //should be nullable
          }
        }
      }
      this.fixtureRepository.SaveChanges();
      return matchIDs;
    }
  }

  public class FootballPredictionAdminService : PredictionService, IFootballPredictionAdminService
  {
    public FootballPredictionAdminService(IPredictionStrategyProvider predictionProvider,
      IPredictionRepository predictionRepository, IFixtureRepository fixtureRepository, IStoredProceduresRepository storedProcRepository)
      : base(predictionProvider, predictionRepository, fixtureRepository, storedProcRepository)
    { }

    public IEnumerable<FootballPredictionViewModel> GetFootballPredictions(IEnumerable<FootballFixtureViewModel> fixtures)
    {
      var predictions = GetFootballPredictionsFromFixtures(fixtures);
      return Mapper.Map<IEnumerable<FootballPrediction>, IEnumerable<FootballPredictionViewModel>>(predictions);
    }

    public IEnumerable<FootballPredictionViewModel> FetchFootballPredictions(IEnumerable<FootballFixtureViewModel> fixtures)
    {
      var predictions = FetchFootballPredictionsFromFixtures(fixtures);
      PersistGenericPredictions(predictions);
      return Mapper.Map<IEnumerable<FootballPrediction>, IEnumerable<FootballPredictionViewModel>>(predictions);
    }

    private IEnumerable<FootballPrediction> GetFootballPredictionsFromFixtures(IEnumerable<FootballFixtureViewModel> fixtures)
    {
      var footballPredictions = new Dictionary<int, FootballPrediction>();
      var matchIDs = new Dictionary<int, string>();
      foreach (var fixture in fixtures)
      {
        var homeTeam = this.fixtureRepository.GetTeamOrPlayerFromName(fixture.HomeTeam);
        var awayTeam = this.fixtureRepository.GetTeamOrPlayerFromName(fixture.AwayTeam);
        var match = this.fixtureRepository.GetMatchFromTeamSelections(homeTeam, awayTeam, fixture.MatchDate.Date);
        var tournamentEvent = this.fixtureRepository.GetTournamentEventById(match.TournamentEventID);
        var tournament = this.fixtureRepository.GetTournamentFromTournamentEvent(tournamentEvent.EventName);

        var identifier = string.Format("{0}/vs/{1}/{2}/{3}", homeTeam.Name, awayTeam.Name, tournamentEvent.EventName, 
          fixture.MatchDate.Date.ToShortDateString().Replace("/", "-"));
        matchIDs.Add(match.Id, identifier);
        footballPredictions.Add(match.Id, new FootballPrediction
        {
          MatchIdentifier = identifier,
          TournamentName = tournament.TournamentName,
          MatchDate = match.MatchDate,
          TeamOrPlayerA = homeTeam.Name,
          TeamOrPlayerB = awayTeam.Name
        });
      }

      var outcomePredictions = this.predictionRepository.GetMatchOutcomeProbabilitiesInMatchByIDs(matchIDs.Keys);
      var scoreLinePredictions = this.predictionRepository.GetScoreOutcomeProbabilitiesInMatchByIDs(matchIDs.Keys);

      foreach (var id in matchIDs.Keys)
      {
        var footballPrediction = footballPredictions[id];

        footballPrediction.OutcomeProbabilities = outcomePredictions[id].ToDictionary(o => (Outcome)o.MatchOutcomeID, o => (double)o.MatchOutcomeProbability);
        footballPrediction.ScoreLineProbabilities = scoreLinePredictions[id].ToDictionary(o => string.Format("{0}-{1}", o.ScoreOutcome.TeamAScore, o.ScoreOutcome.TeamBScore), o => (double?)o.ScoreOutcomeProbability);
      }

      return footballPredictions.Values;
    }

    private IEnumerable<FootballPrediction> FetchFootballPredictionsFromFixtures(IEnumerable<FootballFixtureViewModel> fixtures)
    {
      var predictions = new List<FootballPrediction>();
      var source = this.fixtureRepository.GetExternalSource("Fink Tank (dectech)");
      var football = this.fixtureRepository.GetSport("Football");
      var predictionStrategy = this.predictionProvider.CreatePredictionStrategy(football);

      (from fixture in fixtures
       group fixture by fixture.League into byLeagues
       let tournament = this.fixtureRepository.GetTournamentFromTournamentEvent(fixtures.First(f => f.League == byLeagues.Key).League)
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
                              .SelectMany(f => predictionStrategy.FetchPredictions(f.ValueOptions))
                              .Cast<FootballPrediction>()));

      return predictions;
    }

    
  }
}
