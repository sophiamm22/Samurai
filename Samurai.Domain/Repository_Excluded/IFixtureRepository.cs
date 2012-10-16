using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Entities;

namespace Samurai.Domain.Repository
{
  public interface IFixtureRepository
  {
    Uri GetSkySportsFootballFixturesOrResults(DateTime fixtureDate);
    TeamsPlayer GetTeamFromSkySportsName(string teamName);
    Match GetFootballFixtureFromTeamSelections(TeamsPlayer homeTeam, TeamsPlayer awayTeam, DateTime seasonDate);
    Competition GetCompetition(int competitionID);
    ScoreOutcome GetScoreOutcome(int teamAScore, int teamBScore);
    Match SaveMatch(Match match);
    void SaveChanges();
  }
}
