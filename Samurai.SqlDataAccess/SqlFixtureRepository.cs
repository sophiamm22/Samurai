using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Objects;

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

    public Match GetTennisMatch(string playerA, string playerB, DateTime matchDate)
    {
      return First<Match>(m => m.TeamsPlayerA.TeamName == playerA &&
                              m.TeamsPlayerB.TeamName == playerB &&
                              m.MatchDate == matchDate);
    }

    public IEnumerable<Match> GetDaysTennisMatches(DateTime matchDate)
    {
      return GetQuery<Match>(m => m.MatchDate == matchDate && 
                                  m.TournamentEvent.Tournament.Competition.Sport.SportName == "Tennis");
    }

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

    public IEnumerable<Match> GetDaysFootballMatches(DateTime matchDate)
    {
      var seasonStartYear = matchDate.Month >= 6 ? matchDate.Year : (matchDate.Year - 1);

      return GetQuery<Match>(m => EntityFunctions.TruncateTime(m.MatchDate) == matchDate.Date)
                .Include(m => m.TournamentEvent.Tournament);
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

    public TeamPlayer GetTeamOrPlayer(string slug)
    {
      var teamOrPlayer = First<TeamPlayer>(t => t.Slug == slug);
      return teamOrPlayer;
    }

    public IEnumerable<Match> GetMatchesFromTeamSelections(TeamPlayer homeTeam, TeamPlayer awayTeam, DateTime startDate, DateTime endDate)
    {
      return GetQuery<Match>(m => m.TeamAID == homeTeam.Id &&
                                  m.TeamBID == awayTeam.Id &&
                                  m.MatchDate.Date >= startDate &&
                                  m.MatchDate.Date <= endDate)
                            .Include("ObservedOutcomes")
                            .Include("ObservedOutcomes.ScoreOutcome");
    }

    public Match GetMatchFromTeamSelections(TeamPlayer homeTeam, TeamPlayer awayTeam, DateTime matchDate)
    {
      return GetQuery<Match>(m => m.TeamAID == homeTeam.Id &&
                                  m.TeamBID == awayTeam.Id &&
                                  EntityFunctions.TruncateTime(m.MatchDate) == matchDate.Date)
                            .Include("ObservedOutcomes")
                            .Include("ObservedOutcomes.ScoreOutcome")
                            .Include("TournamentEvent")
                            .Include("TournamentEvent.Tournament")
                            .FirstOrDefault();
    }

    public IEnumerable<Match> GetMatchesForOdds(DateTime matchDate, string tournament)
    {
      return GetQuery<Match>(m => EntityFunctions.TruncateTime(m.MatchDate) == matchDate.Date)
                            .Where(m => m.TournamentEvent.Tournament.TournamentName == tournament)
                            .Include(m => m.MatchCouponURLs.Select(u => u.ExternalSource))
                            .Include(m => m.MatchOutcomeProbabilitiesInMatches.Select(o => o.MatchOutcome))
                            .Include(m => m.MatchOutcomeProbabilitiesInMatches.Select(o => o.MatchOutcomeOdds.Select(e => e.ExternalSource)))
                            .ToList();
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

    public MatchOutcome GetMatchOutcomeByID(int id)
    {
      return First<MatchOutcome>(m => m.Id == id);
    }

    public Sport GetSport(string sport)
    {
      return First<Sport>(s => s.SportName.ToLower() == sport.ToLower());
    }

    public Tournament GetTournamentFromSlug(string slug)
    {
      return First<Tournament>(t => t.Slug == slug.ToLower());
    }

    public Tournament GetTournament(string tournament)
    {
      return First<Tournament>(t => t.TournamentName == tournament);
    }

    public TeamPlayer GetTeamOrPlayerFromName(string team)
    {
      return First<TeamPlayer>(t => t.TeamName == team);
    }

    public void AddMatch(Match match)
    {
      Add<Match>(match);
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
