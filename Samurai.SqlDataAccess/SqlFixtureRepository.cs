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

    public IEnumerable<Match> GetDaysFootballMatches(string competitionName, DateTime matchDate)
    {
      var seasonStartYear = matchDate.Month >= 6 ? matchDate.Year : (matchDate.Year - 1);

      var competition = GetQuery<Competition>(t => t.CompetitionName == competitionName)
                          .Include(t => t.Tournaments)
                          .FirstOrDefault();
      return competition.Tournaments.FirstOrDefault().TournamentEvents
                          .FirstOrDefault(t => t.StartDate.Year == seasonStartYear)
                          .Matches;
    }

    public ExternalSource GetExternalSource(string sourceName)
    {
      return First<ExternalSource>(g => g.Source == sourceName);
    }

    public string GetAlias(string teamNameSource, ExternalSource source, ExternalSource destination)
    {
      var teamNameDestination = string.Empty;
      var teamAlias = GetQuery<TeamPlayerExternalSourceAlias>()
                        .Include(t => t.TeamsPlayer)
                        .Where(a => a.Alias == teamNameSource &&
                                    a.ExternalSource.Source == source.Source);

      if (teamAlias.Count() == 0)
        teamNameDestination = teamNameSource;
      else
        teamNameDestination = teamAlias.First().TeamsPlayer.TeamName;

      return teamNameDestination;
    }

    public Uri GetSkySportsFootballFixturesOrResults(DateTime fixtureDate)
    {
      //should get this from the database
      return new Uri(string.Format(@"http://www1.skysports.com/football/fixtures-results/{0}-{1}-{2}",
        fixtureDate.Day, fixtureDate.ToString("MMMM").ToLower(), fixtureDate.Year));
    }

    public TeamsPlayer GetTeamOrPlayer(string slug)
    {
      var teamOrPlayer = First<TeamsPlayer>(t => t.Slug == slug);
      return teamOrPlayer;
    }

    public IEnumerable<Match> GetMatchesFromTeamSelections(TeamsPlayer homeTeam, TeamsPlayer awayTeam, DateTime startDate, DateTime endDate)
    {
      return GetQuery<Match>(m => m.TeamAID == homeTeam.Id &&
                                  m.TeamBID == awayTeam.Id &&
                                  m.MatchDate.Date >= startDate &&
                                  m.MatchDate.Date <= endDate)
                            .Include("ObservedOutcomes")
                            .Include("ObservedOutcomes.ScoreOutcome");
    }

    public Match GetMatchFromTeamSelections(TeamsPlayer homeTeam, TeamsPlayer awayTeam, DateTime matchDate)
    {
      return GetQuery<Match>(m => m.TeamAID == homeTeam.Id &&
                                  m.TeamBID == awayTeam.Id &&
                                  m.MatchDate.Date == matchDate.Date)
                            .Include("ObservedOutcomes")
                            .Include("ObservedOutcomes.ScoreOutcome")
                            .Include("TournamentEvent")
                            .Include("TournamentEvent.Tournament")
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

    public TournamentEvent GetFootballTournamentEvent(int leagueEnum, DateTime matchDate)
    {
      var seasonStartYear = matchDate.Month >= 6 ? matchDate.Year : (matchDate.Year - 1);

      var tournament = GetQuery<Tournament>(t => t.Id == leagueEnum)
                        .Include(t => t.TournamentEvents)
                        .FirstOrDefault();
      return tournament.TournamentEvents
                       .FirstOrDefault(t => t.StartDate.Year == seasonStartYear);//
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
