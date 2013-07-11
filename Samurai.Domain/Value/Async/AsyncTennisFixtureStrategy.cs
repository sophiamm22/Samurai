using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Samurai.Domain.Entities;
using Samurai.Domain.Infrastructure;
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Domain.APIModel;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Repository;
using Samurai.Domain.Model;
using Samurai.Core;

namespace Samurai.Domain.Value.Async
{
  public interface IAsyncTennisFixtureStrategy
  {
    Task<IEnumerable<TournamentEvent>> UpdateTournamentEvents();
    Task<IEnumerable<GenericMatchDetailQuery>> UpdateResults(DateTime fixtureDate);
    Task<APITournamentDetail> GetTournamentDetail(string tournament, int year);
  }

  public class AsyncTennisFixtureStrategy : IAsyncTennisFixtureStrategy
  {
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly IWebRepositoryProviderAsync webRepositoryProvider;

    public AsyncTennisFixtureStrategy(IFixtureRepository fixtureRepository,
      IWebRepositoryProviderAsync webRepositoryProvider)
    {
      this.fixtureRepository = fixtureRepository;
      this.webRepositoryProvider = webRepositoryProvider;
    }

    public async Task<IEnumerable<TournamentEvent>> UpdateTournamentEvents()
    {
      var ret = new List<TournamentEvent>();
      var tb365Uri = this.fixtureRepository.GetTennisTournamentCalendar();

      var webRepository =
        this.webRepositoryProvider.CreateWebRepository(DateTime.Now.Date);

      var tournamentEvents = await 
        webRepository.ParseJsonEnumerable<APITennisTourCalendar>(tb365Uri);

      foreach (var tournamentEvent in tournamentEvents)
      {
        var nameWithoutYear = Regex.Replace(tournamentEvent.TournamentName, @" 20\d{2}", "");
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

    public async Task<IEnumerable<GenericMatchDetailQuery>> UpdateResults(DateTime fixtureDate)
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
        await webRepository.ParseJsonEnumerable<APIDaysResults>(tb365Uri);

      var daysResultsDictionary =
        daysResults.ToDictionary(x => string.Format("{0},{1} vs. {2},{3}"));

      foreach (var result in daysResults)
      {
        Entities.Match persistedMatch;
        persistedMatch = persistedMatches
          .FirstOrDefault(x => x.TeamsPlayerA.FirstName == result.WinnerFirstName && x.TeamsPlayerA .Name == result.WinnerSurname &&
                               x.TeamsPlayerB.FirstName == result.LoserFirstName  && x.TeamsPlayerB.Name ==  result.LoserSurname);
        if (persistedMatch == null)
          persistedMatch = persistedMatches
          .FirstOrDefault(x => x.TeamsPlayerA.FirstName == result.LoserFirstName && x.TeamsPlayerA.Name == result.LoserSurname &&
                               x.TeamsPlayerB.FirstName == result.WinnerFirstName  && x.TeamsPlayerB.Name == result.WinnerSurname);

        if (persistedMatch == null)
        {
          ProgressReporterProvider.Current.ReportProgress(
            string.Format("Result existed for {0} {1} vs. {2} {3} @ {4} had no persisted match", result.WinnerFirstName, result.WinnerSurname, result.LoserFirstName, result.LoserSurname, result.TournamentName),
            ReporterImportance.Error, ReporterAudience.Admin);
          continue;
        }


        
      }
      throw new NotImplementedException();
    }

    public async Task<APITournamentDetail> GetTournamentDetail(string tournament, int year)
    {
      var tb365Uri = 
        this.fixtureRepository
            .GetTennisTournamentLadder(tournament, year);
      
      var webRepository = 
        this.webRepositoryProvider
            .CreateWebRepository(DateTime.Now.Date);

      var tournamentDetail = 
        await webRepository.ParseJson<APITournamentDetail>(tb365Uri);

      return tournamentDetail;
    }
  }
}
