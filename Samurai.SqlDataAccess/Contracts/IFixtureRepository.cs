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
    TeamsPlayer GetTeamOrPlayer(string slug);
    IEnumerable<Match> GetMatchesFromTeamSelections(TeamsPlayer homeTeam, TeamsPlayer awayTeam, DateTime startDate, DateTime endDate);
    Match GetMatchFromTeamSelections(TeamsPlayer homeTeam, TeamsPlayer awayTeam, DateTime matchDate);
    Competition GetCompetition(int competitionID);
    TournamentEvent GetFootballTournamentEvent(int leagueEnum, DateTime matchDate);
    ScoreOutcome GetScoreOutcome(int teamAScore, int teamBScore);
    Match SaveMatch(Match match);
    void SaveChanges();
  }
}
