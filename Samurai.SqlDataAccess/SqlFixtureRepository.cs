using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Objects;
using System.Data.Objects.SqlClient;

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

    public Match GetTennisMatch(string playerASlug, string playerBSlug, DateTime matchDate)
    {
      return First<Match>(m => m.TeamsPlayerA.Slug == playerASlug &&
                              m.TeamsPlayerB.Slug == playerBSlug &&
                              m.MatchDate == matchDate);
    }

    public IQueryable<Match> GetDaysMatches(DateTime matchDate, string sport)
    {
      return GetQuery<Match>(m => EntityFunctions.TruncateTime(m.MatchDate) == matchDate.Date &&
                                  m.TournamentEvent.Tournament.Competition.Sport.SportName == sport);
    }

    public IQueryable<Match> GetDaysMatchesWithTeamsTournaments(DateTime matchDate, string sport)
    {
      return GetQuery<Match>(m => EntityFunctions.TruncateTime(m.MatchDate) == matchDate.Date &&
                                  m.TournamentEvent.Tournament.Competition.Sport.SportName == sport)
                            .Include(m => m.TeamsPlayerA)
                            .Include(m => m.TeamsPlayerB)
                            .Include(m => m.TournamentEvent);
    }

    public IEnumerable<Match> GetDaysTennisMatches(DateTime matchDate)
    {
      return GetQuery<Match>(m => m.MatchDate == matchDate && 
                                  m.TournamentEvent.Tournament.Competition.Sport.SportName == "Tennis")
                                  .ToList();
    }

    public IEnumerable<Match> GetDaysMatches(string competitionName, DateTime matchDate) //confusing, why did I do this??
    {
      var seasonStartYear = matchDate.Month >= 6 ? matchDate.Year : (matchDate.Year - 1);

      var competition = GetQuery<Competition>(t => t.CompetitionName == competitionName)
                          .Include(t => t.Tournaments)
                          .FirstOrDefault();
      return competition.Tournaments.FirstOrDefault().TournamentEvents
                          .FirstOrDefault(t => t.StartDate.Year == seasonStartYear)
                          .Matches
                          .Where(m => Convert.ToDateTime(m.MatchDate).ToString("yyyy-MM-dd") == Convert.ToDateTime(matchDate.Date).ToString("yyyy-MM-dd"))
                          .ToList();
    }

    public IEnumerable<Match> GetDaysMatches(DateTime matchDate)
    {
      var seasonStartYear = matchDate.Month >= 6 ? matchDate.Year : (matchDate.Year - 1);

      return GetQuery<Match>(m => EntityFunctions.TruncateTime(m.MatchDate) == matchDate.Date)
                .Include(m => m.TournamentEvent.Tournament)
                .ToList();
    }

    public IEnumerable<Tournament> GetDaysTournaments(DateTime matchDate, string sport)
    {
      var tournaments = GetQuery<TournamentEvent>(t => EntityFunctions.AddDays(t.StartDate, -2) <= matchDate &&
                                                      EntityFunctions.AddDays(t.EndDate, 2) >= matchDate &&
                                                      t.Tournament.Competition.Sport.SportName == sport
        /* &&
        t.TournamentInProgress*/)
                                                      .Select(t => t.Tournament)
                                                      .ToList();
      return tournaments;
    }

    public ExternalSource GetExternalSource(string sourceName)
    {
      return First<ExternalSource>(g => g.Source == sourceName);
    }

    public string GetAlias(string teamNameSource, ExternalSource source, ExternalSource destination, Sport sport)
    {
      if (sport.SportName == "Football")
        return GetAliasFootball(teamNameSource, source, destination);
      else if (sport.SportName == "Tennis")
        return GetAliasTennis(teamNameSource, source, destination);
      else
        throw new ArgumentException(string.Format("Can't find sport named {0}", sport.SportName));
    }

    string GetAliasFootball(string teamNameSource, ExternalSource source, ExternalSource destination)
    {
      //easy, we will know all of these upfront
      var teamNameDestination = string.Empty;
      var teamAlias = GetQuery<TeamPlayerExternalSourceAlias>()
                        .Include(t => t.TeamsPlayer)
                        .Where(a => a.Alias == teamNameSource &&
                                    a.ExternalSource.Source == source.Source);

      if (teamAlias.Count() == 0)
        teamNameDestination = teamNameSource;
      else
        teamNameDestination = teamAlias.First().TeamsPlayer.Name;

      return teamNameDestination;
    }

    private string GetAliasTennis(string teamNameSource, ExternalSource source, ExternalSource destination)
    {
      var teamPlayerAliasLookup = GetQuery<TeamPlayerExternalSourceAlias>(t => t.Alias == teamNameSource && t.ExternalSource.Source == source.Source)
                                    .Include(t => t.TeamsPlayer)
                                    .FirstOrDefault();
      if (teamPlayerAliasLookup != null)
      {
        return teamPlayerAliasLookup.TeamsPlayer.Slug;
      }
      else
      {

        var lookup = StandardisePlayerName(source.Source, teamNameSource);
        TeamPlayer player = null;
        if (source.Source == "Best Betting")
        {
          player = First<TeamPlayer>(p => p.FirstName.ToLower().Substring(0, 1) + "-" + p.Name.ToLower().Replace(" ", "-").Replace(".", "") == lookup);
          if (player == null) return string.Empty;
        }
        else if (source.Source == "Odds Checker Web" || source.Source == "Odds Checker Mobi")
        {
          player = First<TeamPlayer>(p => p.FirstName.ToLower().Replace(" ", "-").Replace(".", "") + "-" + p.Name.Replace(" ", "-").Replace(".", "").ToLower() == lookup);
          if (player == null) return string.Empty;
        }
        else
        {
          throw new ArgumentException("source");
        }

        var teamPlayerAlias = new TeamPlayerExternalSourceAlias
        {
          ExternalSource = source,
          Alias = teamNameSource,
          TeamsPlayer = player
        };

        Add<TeamPlayerExternalSourceAlias>(teamPlayerAlias);
        SaveChanges();
        return player.Slug;
      }
    }

    private string StandardisePlayerName(string source, string playerName)
    {
      var returnString = string.Empty;
      if (source == "Best Betting")
      {
        var teamOrPlayerArray = playerName.Split(',');
        returnString = (teamOrPlayerArray[1].Trim().Substring(0, 1) + "-" + teamOrPlayerArray[0].Trim()).RemoveDiacritics().Replace(' ','-').ToLower();
        //like: r-federer
      }
      else if (source == "Odds Checker Web" || source == "Odds Checker Mobi")
      {
        returnString = playerName.RemoveDiacritics().Replace(' ', '-').ToLower();
        //like: roger-federer
      }
      else
        throw new ArgumentException("source");      

      return returnString;
    }

    public Uri GetSkySportsFootballFixturesOrResults(DateTime fixtureDate)
    {
      //should get this from the database
      return new Uri(string.Format(@"http://www1.skysports.com/football/fixtures-results/{0}-{1}-{2}",
        fixtureDate.Day.ToString("00"), fixtureDate.ToString("MMMM").ToLower(), fixtureDate.Year));
    }

    public TeamPlayer GetTeamOrPlayerById(int id)
    {
      var teamOrPlayer = GetByKey<TeamPlayer>(id);
      return teamOrPlayer;
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
                            .Include("ObservedOutcomes.ScoreOutcome")
                            .ToList();
    }

    public Match GetMatchFromTeamSelections(TeamPlayer homeTeam, TeamPlayer awayTeam, DateTime matchDate)
    {
      var match = GetQuery<Match>(m => m.TeamAID == homeTeam.Id &&
                                  m.TeamBID == awayTeam.Id &&
                                  EntityFunctions.TruncateTime(m.MatchDate) == matchDate.Date)
                            .FirstOrDefault();
      return match;
    }

    public TournamentEvent GetTournamentEventFromTournamentAndDate(DateTime matchDate, string tournamentName)
    {
      var tournamentEvent = GetQuery<TournamentEvent>(t => t.Tournament.TournamentName == tournamentName)
                              .OrderBy(t => SqlFunctions.DateDiff("dd", t.StartDate, matchDate) ^ 2)
                              .First();
      return tournamentEvent;
    }

    public Match CreateMatch(TeamPlayer homeTeam, TeamPlayer awayTeam, DateTime matchDate, TournamentEvent tournamentEvent)
    {
      var match = new Match()
      {
        TeamsPlayerA = homeTeam,
        TeamsPlayerB = awayTeam,
        MatchDate = matchDate,
        TournamentEvent = tournamentEvent
      };
      AddMatch(match);
      Save<Match>(match);//need ID
      return match;
    }

    public IEnumerable<Match> GetMatchesForTournament(DateTime matchDate, string tournament)
    {
      return GetQuery<Match>(m => EntityFunctions.TruncateTime(m.MatchDate) == matchDate.Date)
                      .Where(m => m.TournamentEvent.Tournament.TournamentName == tournament)
                      .ToList();
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
      return GetByKey<Competition>(competitionID);
    }

    public TournamentEvent GetTournamentEventById(int tournamentEventID)
    {
      return GetByKey<TournamentEvent>(tournamentEventID);
    }

    public TournamentEvent GetFootballTournamentEvent(int leagueEnum, DateTime matchDate)
    {
      var seasonStartYear = matchDate.Month >= 6 ? matchDate.Year : (matchDate.Year - 1);

      var tournament = GetQuery<Tournament>(t => t.Id == leagueEnum)
                        .Include(t => t.TournamentEvents)
                        .Include(t => t.Competition)
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

    public Tournament GetTournamentFromTournamentEvent(string tournamentEventName)
    {
      var tournamentEvent = GetQuery<TournamentEvent>(t => t.EventName == tournamentEventName)
                            .Include(t=>t.Tournament)
                            .FirstOrDefault();

      return tournamentEvent.Tournament;
    }

    public Tournament GetTournament(string tournament)
    {
      return First<Tournament>(t => t.TournamentName == tournament);
    }

    public Tournament GetTournamentByID(int tournamentID)
    {
      return GetByKey<Tournament>(tournamentID);
    }

    public TeamPlayer GetTeamOrPlayerFromName(string team)
    {
      return First<TeamPlayer>(t => t.Name == team);
    }

    public TeamPlayer GetTeamOrPlayerFromNameAndMaybeFirstName(string teamSurname, string firstName)
    {
      var teamOrPlayer = string.IsNullOrEmpty(firstName) ? First<TeamPlayer>(t => t.Name == teamSurname) : First<TeamPlayer>(t => t.Name == teamSurname && t.FirstName == firstName);
      if (teamOrPlayer == null)
      {
        teamOrPlayer = new TeamPlayer() { Name = teamSurname };
        if (string.IsNullOrEmpty(firstName))
        {
          teamOrPlayer.Slug = teamSurname.ToLower().RemoveDiacritics();
        }
        else
        {
          teamOrPlayer.Slug = string.Format("{0}-{1}", firstName, teamSurname).RemoveDiacritics().RemoveSpecialCharacters().ToLower().Replace(' ', '-');
          teamOrPlayer.FirstName = firstName;
        }
        Add<TeamPlayer>(teamOrPlayer);
        SaveChanges();
      }
      return teamOrPlayer;
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
