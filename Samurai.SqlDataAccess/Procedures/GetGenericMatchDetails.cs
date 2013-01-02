using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Objects;

using Infrastructure.Data;
using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;

namespace Samurai.SqlDataAccess.Procedures
{
  public partial class SqlStoredProceduresRepository
  {
    public IQueryable<GenericMatchDetailQuery> GetGenericMatchDetails(DateTime matchDate, string queriedSport)
    {
      var matches =
              from match in DbSet<Match>()
              join homeTeam in DbSet<TeamPlayer>() on match.TeamAID equals homeTeam.Id
              join awayTeam in DbSet<TeamPlayer>() on match.TeamBID equals awayTeam.Id
              join tournamentEvent in DbSet<TournamentEvent>() on match.TournamentEventID equals tournamentEvent.Id
              join tournament in DbSet<Tournament>() on tournamentEvent.TournamentID equals tournament.Id
              join competition in DbSet<Competition>() on tournament.CompetitionID equals competition.Id
              join sport in DbSet<Sport>() on competition.SportID equals sport.Id
              join observedOutcome in DbSet<ObservedOutcome>() on match.Id equals observedOutcome.MatchID into joinedObservedOutcome
              from observedOutcome in joinedObservedOutcome.DefaultIfEmpty()
              join scoreOutcome in DbSet<ScoreOutcome>() on observedOutcome.ScoreOutcomeID equals scoreOutcome.Id into joinedScoreOutcome
              from scoreOutcome in joinedScoreOutcome.DefaultIfEmpty()

              where (EntityFunctions.TruncateTime(match.MatchDate) == matchDate.Date && sport.SportName == queriedSport)
              select new GenericMatchDetailQuery
              {
                MatchID = match.Id,
                MatchDate = match.MatchDate,
                TournamentID = tournament.Id,
                TournamentName = tournament.TournamentName,
                TournamentEventID = tournamentEvent.Id,
                TournamentEventName = tournamentEvent.EventName,

                CompetitionID = competition.Id,
                CompetitionName = competition.CompetitionName,

                PlayerAID = homeTeam.Id,
                TeamOrPlayerA = homeTeam.Name,
                PlayerAFirstName = homeTeam.FirstName,

                PlayerBID = awayTeam.Id,
                TeamOrPlayerB = awayTeam.Name,
                PlayerBFirstName = awayTeam.FirstName,

                ScoreOutcomeID = scoreOutcome != null ? scoreOutcome.Id : new Nullable<int>(),
                ScoreAHack = scoreOutcome != null ? scoreOutcome.TeamAScore : -1,
                ScoreBHack = scoreOutcome != null ? scoreOutcome.TeamBScore : -1
              };
      return matches;

    }
  }
}
