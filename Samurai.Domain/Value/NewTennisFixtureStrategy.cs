﻿using System;
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

namespace Samurai.Domain.Value
{
  public interface ITennisFixtureStrategy
  {
    IEnumerable<TournamentEvent> UpdateTournamentEvents();
    IEnumerable<GenericMatchDetailQuery> UpdateResultsNew(DateTime fixtureDate);
    APITournamentDetail GetTournamentDetail(string tournament, int year);
  }

  public class NewTennisFixtureStrategy : ITennisFixtureStrategy
  {
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly IStoredProceduresRepository storedProcRepository;
    protected readonly IWebRepositoryProvider webRepositoryProvider;

    public NewTennisFixtureStrategy(IFixtureRepository fixtureRepository, IStoredProceduresRepository storedProcRepository,
      IWebRepositoryProvider webRepositoryProvider)
    {
      this.fixtureRepository = fixtureRepository;
      this.storedProcRepository = storedProcRepository;
      this.webRepositoryProvider = webRepositoryProvider;
    }

    public IEnumerable<TournamentEvent> UpdateTournamentEvents()
    {
      var ret = new List<TournamentEvent>();
      var tb365Uri = this.fixtureRepository.GetTennisTournamentCalendar();

      var webRepository = this.webRepositoryProvider.CreateWebRepository(DateTime.Now.Date);

      var tournamentEvents = webRepository.GetJsonObjects<APITennisTourCalendar>(tb365Uri, s => Console.WriteLine(s));

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
            Slug = tournamentEvent.TournamentName.RemoveDiacritics().ToHyphenated(),
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


    public IEnumerable<GenericMatchDetailQuery> UpdateResultsNew(DateTime fixtureDate)
    {
      throw new NotImplementedException();
    }

    public APITournamentDetail GetTournamentDetail(string tournament, int year)
    {
      var tb365Uri = this.fixtureRepository.GetTennisTournamentLadder(tournament, year);
      var webRepository = this.webRepositoryProvider.CreateWebRepository(DateTime.Now.Date);

      var tournamentDetail = webRepository.GetJsonObject<APITournamentDetail>(tb365Uri, s => Console.WriteLine(s));

      return tournamentDetail;
    }
  }
}