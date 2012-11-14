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
    IEnumerable<Match> GetDaysFootballMatches(string competition, DateTime matchDate);
    ExternalSource GetExternalSource(string sourceName);
    string GetAlias(string teamName, ExternalSource source, ExternalSource destination);
    Uri GetSkySportsFootballFixturesOrResults(DateTime fixtureDate);
    TeamPlayer GetTeamOrPlayer(string slug);
    TeamPlayer GetTeamOrPlayerFromName(string team);
    IEnumerable<Match> GetMatchesFromTeamSelections(TeamPlayer homeTeam, TeamPlayer awayTeam, DateTime startDate, DateTime endDate);
    Match GetMatchFromTeamSelections(TeamPlayer homeTeam, TeamPlayer awayTeam, DateTime matchDate);
    IEnumerable<Match> GetMatchesForOdds(DateTime matchDate);
    Competition GetCompetition(int competitionID);
    TournamentEvent GetFootballTournamentEvent(int leagueEnum, DateTime matchDate);
    ScoreOutcome GetScoreOutcome(int teamAScore, int teamBScore);
    MatchOutcome GetMatchOutcomeByID(int id);
    Match SaveMatch(Match match);
    void SaveChanges();
    Sport GetSport(string sport);
    Tournament GetTournament(string tournament);
    Tournament GetTournamentFromSlug(string slug);
  }
}
