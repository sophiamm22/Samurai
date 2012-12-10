﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Contracts
{
  public interface IFixtureRepository
  {
    Match GetTennisMatch(string playerASlug, string playerBSlug, DateTime matchDate);
    IEnumerable<Match> GetDaysTennisMatches(DateTime matchDate);
    IEnumerable<Match> GetDaysMatches(string competition, DateTime matchDate);
    IEnumerable<Match> GetDaysMatches(DateTime matchDate);
    IEnumerable<Tournament> GetDaysTournaments(DateTime matchDate);
    ExternalSource GetExternalSource(string sourceName);
    string GetAlias(string teamName, ExternalSource source, ExternalSource destination, Sport sport);
    Uri GetSkySportsFootballFixturesOrResults(DateTime fixtureDate);
    TeamPlayer GetTeamOrPlayer(string slug);
    TeamPlayer GetTeamOrPlayerFromName(string team);
    TeamPlayer GetTeamOrPlayerFromNameAndMaybeFirstName(string teamSurname, string firstName);
    IEnumerable<Match> GetMatchesFromTeamSelections(TeamPlayer homeTeam, TeamPlayer awayTeam, DateTime startDate, DateTime endDate);
    Match GetMatchFromTeamSelections(TeamPlayer homeTeam, TeamPlayer awayTeam, DateTime matchDate);
    TournamentEvent GetTournamentEventFromTournamentAndDate(DateTime matchDate, string tournamentName);
    Match CreateMatch(TeamPlayer homeTeam, TeamPlayer awayTeam, DateTime matchDate, TournamentEvent tournamentEvent);
    IEnumerable<Match> GetMatchesForOdds(DateTime matchDate, string tournament);
    Competition GetCompetition(int competitionID);
    TournamentEvent GetFootballTournamentEvent(int leagueEnum, DateTime matchDate);
    ScoreOutcome GetScoreOutcome(int teamAScore, int teamBScore);
    MatchOutcome GetMatchOutcomeByID(int id);
    Match SaveMatch(Match match);
    void SaveChanges();
    void AddMatch(Match match);
    Sport GetSport(string sport);
    Tournament GetTournament(string tournament);
    Tournament GetTournamentFromSlug(string slug);
  }
}
