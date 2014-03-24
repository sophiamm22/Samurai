using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Contracts
{
  public interface IFixtureRepository
  {
    Match GetMatchFromTeamSelections(TeamPlayer homeTeam, TeamPlayer awayTeam, DateTime matchDate);
    Match CreateMatch(TeamPlayer homeTeam, TeamPlayer awayTeam, DateTime matchDate, TournamentEvent tournamentEvent);
    Match SaveMatch(Match match);
    Match GetTennisMatch(string playerASlug, string playerBSlug, DateTime matchDate);
    IQueryable<Match> GetDaysMatches(DateTime matchDate, string sport);
    IQueryable<Match> GetDaysMatchesWithTeamsTournaments(DateTime matchDate, string sport);
    IEnumerable<Match> GetMatchesFromTeamSelections(TeamPlayer homeTeam, TeamPlayer awayTeam, DateTime startDate, DateTime endDate);
    IEnumerable<Match> GetMatchesForOdds(DateTime matchDate, string tournament);
    IEnumerable<Match> GetMatchesForTournament(DateTime matchDate, string tournament);
    IEnumerable<Match> GetDaysTennisMatches(DateTime matchDate);
    IEnumerable<Match> GetDaysMatches(string competition, DateTime matchDate);
    IEnumerable<Match> GetDaysMatches(DateTime matchDate);
    
    ExternalSource GetExternalSource(string sourceName);
    TeamPlayer GetAlias(string teamName, ExternalSource source, ExternalSource destination, Sport sport);
    TeamPlayerExternalSourceAlias CreateTeamPlayerExternalAlias(TeamPlayer teamPlayer, ExternalSource source, string alias);
    
    Uri GetSkySportsFootballFixturesOrResults(DateTime fixtureDate);
    Uri GetTennisTournamentCalendar();
    Uri GetTennisTournamentLadder(string tournamentName, int year);
    Uri GetDaysResultsURI(DateTime fixtureDate);
    
    TeamPlayer GetTeamOrPlayerById(int id);
    TeamPlayer GetTeamOrPlayer(string slug);
    TeamPlayer GetTeamOrPlayerFromName(string team);
    TeamPlayer GetTeamOrPlayerFromNameAndMaybeFirstName(string teamSurname, string firstName);
    IQueryable<TeamPlayer> GetLeagueLadder(string leagueName, DateTime date);
    void AddMissingTeamPlayerAlias(IEnumerable<MissingTeamPlayerExternalSourceAlias> aliass);
    
    void AddMatch(Match match);
    
    Tournament GetTournamentFromTournamentEvent(string tournamentEventName);
    Tournament CreateTournament(Tournament entity);
    Tournament GetTournament(string tournament);
    Tournament GetTournamentByID(int tournamentID);
    Tournament GetTournamentFromSlug(string slug);
    IEnumerable<Tournament> GetDaysTournaments(DateTime matchDate, string sport);

    void AddTournamentEvent(TournamentEvent entity);
    TournamentEvent GetTournamentEventFromTournamentAndDate(DateTime matchDate, string tournamentName);
    TournamentEvent GetTournamentEventFromTournamentEventNameAndDate(DateTime matchDate, string tournamentName);
    TournamentEvent GetTournamentEventFromTournamentAndYear(int year, string tournamentName);
    TournamentEvent GetFootballTournamentEvent(int leagueEnum, DateTime matchDate);
    TournamentEvent GetTournamentEventById(int tournamentEventID);

    Competition GetCompetitionById(int competitionID);
    Competition GetCompetition(string competitionName);

    ScoreOutcome GetScoreOutcome(int teamAScore, int teamBScore, bool? teamPlayerAWins = null);

    void AddOrUpdateObservedOutcome(ObservedOutcome observedOutcome);

    MatchOutcome GetMatchOutcomeByID(int id);

    Sport GetSport(string sport);

    DateTime GetLatestDate();

    void SaveChanges();
  }
}
