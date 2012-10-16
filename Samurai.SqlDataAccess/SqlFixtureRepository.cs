using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using Infrastructure.Data;
using Samurai.Domain.Entities;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.SqlDataAccess
{
  public class SqlFixtureRepository : GenericRepository, IFixtureRepository
  {
    public SqlFixtureRepository(DbContext context)
      : base(context)
    { }

    public Uri GetSkySportsFootballFixturesOrResults(DateTime fixtureDate)
    {
      //should get this from the database
      return new Uri(string.Format(@"http://www1.skysports.com/football/fixtures-results/{0}-{1}-{2}",
        fixtureDate.Day, fixtureDate.ToString("MMMM").ToLower(), fixtureDate.Year));
    }

    public TeamsPlayer GetTeamFromSkySportsName(string teamName)
    {
      var externalSource = GetQuery<ExternalSource>(x => x.Source == "Sky Sports")
        .Include("TeamPlayerExternalSourceAlias")
        .Include("TeamPlayerExternalSourceAlias.TeamsPlayer");

      var teamOrPlayer = externalSource.SelectMany(x => x.TeamPlayerExternalSourceAlias)
                                       .Where(a => a.Alias == teamName)
                                       .Select(a => a.TeamsPlayer)
                                       .FirstOrDefault();
      return teamOrPlayer;
    }

    public Match GetFootballFixtureFromTeamSelections(TeamsPlayer homeTeam, TeamsPlayer awayTeam, DateTime seasonDate)
    {
      var seasonStartYear = seasonDate.Month >= 8 ? seasonDate.Year : (seasonDate.Year - 1);
      var seasonEndYear = seasonDate.Month >= 8 ? (seasonDate.Year + 1) : seasonDate.Year;
      var safeDateStart = new DateTime(seasonStartYear, 8, 1);
      var safeDateEnd = new DateTime(seasonEndYear, 5, 31);

      return GetQuery<Match>(m => m.TeamAID == homeTeam.Id &&
                                  m.TeamBID == awayTeam.Id &&
                                  m.MatchDate.Date >= safeDateStart &&
                                  m.MatchDate.Date <= safeDateEnd)
                            .Include("ObservedOutcomes")
                            .Include("ObservedOutcomes.ScoreOutcome")
                            .FirstOrDefault();
    }

    public ScoreOutcome GetScoreOutcome(int teamAScore, int teamBScore)
    {
      return First<ScoreOutcome>(o => o.TeamAScore == teamAScore && o.TeamBScore == teamBScore);
    }

    public Competition GetCompetition(int competitionID)
    {
      return First<Competition>(l => l.Id == competitionID);
    }

    public Match SaveMatch(Match match)
    {
      return Save<Match>(match);
    }

    public void SaveChanges()
    {
      UnitOfWork.SaveChanges();
    }
    

  }
}
