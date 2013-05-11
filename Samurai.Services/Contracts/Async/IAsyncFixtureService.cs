﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;
using Samurai.Web.ViewModels.Tennis;

namespace Samurai.Services.Contracts.Async
{
  public interface IAsyncFixtureService
  {
    int GetCountOfDaysMatches(DateTime fixtureDate, string sport);
    TournamentViewModel GetTournamentFromSlug(string slug);
    TournamentViewModel GetTournament(string tournamentName);
    TeamPlayerViewModel GetTeamOrPlayer(string slug);
    void AddAlias(string source, string playerName, string valueSamuraiName, string valueSamuraiFirstName = null);
    DateTime GetLatestDate();
  }

  public interface IAsyncFootballFixtureService : IAsyncFixtureService
  {
    FootballFixtureViewModel GetFootballFixture(DateTime fixtureDate, string homeTeam, string awayTeam);
    Task<IEnumerable<FootballFixtureViewModel>> FetchSkySportsFootballFixtures(DateTime fixtureDate);
    Task<IEnumerable<FootballFixtureViewModel>> FetchSkySportsFootballResults(DateTime fixtureDate);
    IEnumerable<FootballFixtureViewModel> GetFootballFixturesByDate(DateTime fixtureDate);
    IEnumerable<FootballFixtureViewModel> GetFootballFixturesByDateLeague(DateTime fixtureDate, string league);
    IEnumerable<FootballFixtureViewModel> GetFootballFixturesByGameweek(int gameWeek, string league);
    IEnumerable<FootballLadderViewModel> GetTournamentLadder(DateTime matchDate, string tournament);
  }

  public interface IAsyncTennisFixtureService : IAsyncFixtureService
  {
    IEnumerable<TennisMatchViewModel> GetTennisMatches(DateTime matchDate);
    TennisMatchViewModel GetTennisMatch(string playerAName, string playerBName, DateTime matchDate);
    Task<IEnumerable<TennisMatchViewModel>> FetchTennisResults(DateTime matchDate);
    Task<IEnumerable<TournamentEventViewModel>> FetchTournamentEvents();
    Task<IEnumerable<TennisLadderViewModel>> FetchTournamentLadder(DateTime matchDate, string tournament);
  }
}
