using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reg = System.Text.RegularExpressions;

using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Domain.APIModel;
using Samurai.Core;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Model;
using Samurai.Domain.Infrastructure;

namespace Samurai.Domain.Value
{
  public interface ITennisFixtureStrategy
  {
    IEnumerable<TournamentEvent> UpdateTournamentEvents();
    IEnumerable<GenericMatchDetailQuery> UpdateResults(DateTime fixtureDate);
    APITournamentDetail GetTournamentDetail(string tournament, int year);
  }

  public class TennisFixtureStrategy : ITennisFixtureStrategy
  {
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly IStoredProceduresRepository storedProcRepository;
    protected readonly IWebRepositoryProvider webRepositoryProvider;

    public TennisFixtureStrategy(IFixtureRepository fixtureRepository,
      IStoredProceduresRepository storedProcRepository, IWebRepositoryProvider webRepositoryProvider)
    {
      this.fixtureRepository = fixtureRepository;
      this.storedProcRepository = storedProcRepository;
      this.webRepositoryProvider = webRepositoryProvider;
    }

    public IEnumerable<TournamentEvent> UpdateTournamentEvents()
    {
      var ret = new List<TournamentEvent>();
      var tb365Uri = this.fixtureRepository.GetTennisTournamentCalendar();

      var webRepository = 
        this.webRepositoryProvider.CreateWebRepository(DateTime.Now.Date);

      var tournamentEvents = 
        webRepository.GetJsonObjects<APITennisTourCalendar>(tb365Uri, s => ProgressReporterProvider.Current.ReportProgress(s, ReporterImportance.Medium, ReporterAudience.Admin));

      foreach (var tournamentEvent in tournamentEvents)
      {
        var nameWithoutYear = Reg.Regex.Replace(tournamentEvent.TournamentName, @" 20\d{2}", "");
        var tournament = this.fixtureRepository.GetTournament(nameWithoutYear);
        if (tournament == null)
        {
          tournament = new Tournament()
          {
            TournamentName = nameWithoutYear,
            CompetitionID = this.fixtureRepository.GetCompetition("ATP").Id,
            Slug = nameWithoutYear.RemoveDiacritics().ToHyphenated(),
            Location = "Add later"
          };
          this.fixtureRepository.CreateTournament(tournament);
        }
        var eventName = string.Format("{0} ({1})", nameWithoutYear, tournamentEvent.StartDate.AddDays(3).Year);
        var persistedTournamentEvent = this.fixtureRepository.GetTournamentEventFromTournamentAndYear(tournamentEvent.StartDate.AddDays(3).Year, eventName);
        if (persistedTournamentEvent == null)
        {
          persistedTournamentEvent = new TournamentEvent
          {
            EventName = eventName,
            TournamentID = tournament.Id,
            StartDate = tournamentEvent.StartDate,
            EndDate = tournamentEvent.EndDate,
            Slug = string.Format("{0}-{1}", tournamentEvent.TournamentName.RemoveDiacritics().ToHyphenated(), tournamentEvent.StartDate.AddDays(3).Year),
            TournamentInProgress = tournamentEvent.InProgress,
            TournamentCompleted = tournamentEvent.Completed
          };
          this.fixtureRepository.AddTournamentEvent(persistedTournamentEvent);
        }
        else
        {
          persistedTournamentEvent.StartDate = tournamentEvent.StartDate;
          persistedTournamentEvent.EndDate = tournamentEvent.EndDate;
          persistedTournamentEvent.TournamentInProgress = tournamentEvent.InProgress;
          persistedTournamentEvent.TournamentCompleted = tournamentEvent.Completed;
        }

        ret.Add(persistedTournamentEvent);

        this.fixtureRepository.SaveChanges();
      }
      return ret;
    }


    public IEnumerable<GenericMatchDetailQuery> UpdateResults(DateTime fixtureDate)
    {
      var tb365Uri =
        this.fixtureRepository
            .GetDaysResultsURI(fixtureDate);

      var webRepository =
        this.webRepositoryProvider
            .CreateWebRepository(fixtureDate);

      var persistedMatches =
        this.fixtureRepository
            .GetDaysMatchesWithTeamsTournaments(fixtureDate, "Tennis")
            .ToList();

      var daysResults =
          webRepository.GetJsonObjects<APIDaysResults>(tb365Uri, s => { return; });

      foreach (var result in daysResults)
      {
        bool playerAWins = true;
        Entities.Match persistedMatch;
        persistedMatch = persistedMatches
          .FirstOrDefault(x => x.TeamsPlayerA.FirstName == result.WinnerFirstName && x.TeamsPlayerA.Name == result.WinnerSurname &&
                               x.TeamsPlayerB.FirstName == result.LoserFirstName && x.TeamsPlayerB.Name == result.LoserSurname);
        if (persistedMatch == null)
        {
          persistedMatch = persistedMatches
          .FirstOrDefault(x => x.TeamsPlayerA.FirstName == result.LoserFirstName && x.TeamsPlayerA.Name == result.LoserSurname &&
                               x.TeamsPlayerB.FirstName == result.WinnerFirstName && x.TeamsPlayerB.Name == result.WinnerSurname);
          playerAWins = false;
        }

        if (persistedMatch == null)
        {
          ProgressReporterProvider.Current.ReportProgress(
            string.Format("No result for {0} {1} vs. {2} {3} @ {4} had no persisted match", result.WinnerFirstName, result.WinnerSurname, result.LoserFirstName, result.LoserSurname, result.TournamentName),
            ReporterImportance.High, ReporterAudience.Admin);
          continue;
        }

        int playerAScore = 0;
        int playerBScore = 0;
        var setsPlayed = ((result.WinnerFirstSetScore.HasValue && result.LoserFirstSetScore.HasValue) ? 1 : 0) +
                         ((result.WinnerSecondSetScore.HasValue && result.LoserSecondSetScore.HasValue) ? 1 : 0) +
                         ((result.WinnerThirdSetScore.HasValue && result.LoserThirdSetScore.HasValue) ? 1 : 0) +
                         ((result.WinnerFourthSetScore.HasValue && result.LoserFourthSetScore.HasValue) ? 1 : 0) +
                         ((result.WinnerFifthSetScore.HasValue && result.LoserFifthSetScore.HasValue) ? 1 : 0);

        if (setsPlayed >= 1)
        {
          playerAScore += (result.WinnerFirstSetScore.Value > result.LoserFirstSetScore ? 1 : 0);
          playerBScore += (result.WinnerFirstSetScore.Value > result.LoserFirstSetScore ? 0 : 1);
        }
        if (setsPlayed >= 2)
        {
          playerAScore += (result.WinnerSecondSetScore.Value > result.LoserSecondSetScore ? 1 : 0);
          playerBScore += (result.WinnerSecondSetScore.Value > result.LoserSecondSetScore ? 0 : 1);
        }
        if (setsPlayed >= 3)
        {
          playerAScore += (result.WinnerThirdSetScore.Value > result.LoserThirdSetScore ? 1 : 0);
          playerBScore += (result.WinnerThirdSetScore.Value > result.LoserThirdSetScore ? 0 : 1);
        }
        if (setsPlayed >= 4)
        {
          playerAScore += (result.WinnerFourthSetScore.Value > result.LoserFourthSetScore ? 1 : 0);
          playerBScore += (result.WinnerFourthSetScore.Value > result.LoserFourthSetScore ? 0 : 1);
        }
        if (setsPlayed >= 5)
        {
          playerAScore += (result.WinnerFifthSetScore.Value > result.LoserFifthSetScore ? 1 : 0);
          playerBScore += (result.WinnerFifthSetScore.Value > result.LoserFifthSetScore ? 0 : 1);
        }

        var scoreOutcome =
          this.fixtureRepository
              .GetScoreOutcome(playerAScore, playerBScore, playerAWins);

        int commentID = 1;
        if (result.LoserRetired)
          commentID = 2;
        else if (result.LoserWalkedOver)
          commentID = 3;

        //need to check if this already exists
        var observedOutcome = new ObservedOutcome()
        {
          MatchID = persistedMatch.Id,
          ScoreOutcomeID = scoreOutcome.Id,
          OutcomeCommentID = commentID
        };

        this.fixtureRepository.AddOrUpdateObservedOutcome(observedOutcome);

        ProgressReporterProvider.Current.ReportProgress(
          string.Format("Persisted result for {0} {1} vs. {2} {3} @ {4}", result.WinnerFirstName, result.WinnerSurname, result.LoserFirstName, result.LoserSurname, result.TournamentName),
          ReporterImportance.Medium, ReporterAudience.Admin);
      }

      return this.storedProcRepository
                 .GetGenericMatchDetails(fixtureDate, "Tennis")
                 .ToList();
    }

    public APITournamentDetail GetTournamentDetail(string tournament, int year)
    {
      var tb365Uri = this.fixtureRepository.GetTennisTournamentLadder(tournament, year);
      var webRepository = this.webRepositoryProvider.CreateWebRepository(DateTime.Now.Date);

      var tournamentDetail = webRepository.GetJsonObject<APITournamentDetail>(tb365Uri, s => ProgressReporterProvider.Current.ReportProgress(s, ReporterImportance.Low, ReporterAudience.Admin));

      return tournamentDetail;
    }
  }
}
