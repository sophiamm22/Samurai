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
    public IEnumerable<OddsForEvent> GetAllOddsForEvent(DateTime matchDate, string teamA, string teamB)
    {
      return GetOddsForEvent(matchDate, teamA, teamB, true).ToList();
    }

    public IEnumerable<OddsForEvent> GetLatestOddsForEvent(DateTime matchDate, string teamA, string teamB)
    {
      return GetOddsForEvent(matchDate, teamA, teamB, false).ToList();
    }

    private IQueryable<OddsForEvent> GetOddsForEvent(DateTime matchDate, string teamA, string teamB, bool takeAll)
    {
      var matches =
              from match in DbSet<Match>()
              join homeTeam in DbSet<TeamPlayer>() on match.TeamAID equals homeTeam.Id
              join awayTeam in DbSet<TeamPlayer>() on match.TeamBID equals awayTeam.Id
              join tournamentEvent in DbSet<TournamentEvent>() on match.TournamentEventID equals tournamentEvent.Id
              join tournament in DbSet<Tournament>() on tournamentEvent.TournamentID equals tournament.Id
              join probability in DbSet<MatchOutcomeProbabilitiesInMatch>() on match.Id equals probability.MatchID
              join outcome in DbSet<MatchOutcome>() on probability.MatchOutcomeID equals outcome.Id
              join matchOdd in DbSet<MatchOutcomeOdd>() on probability.Id equals matchOdd.MatchOutcomeProbabilitiesInMatchID
              join bookmaker in DbSet<Bookmaker>() on matchOdd.BookmakerID equals bookmaker.Id
              join source in DbSet<ExternalSource>() on matchOdd.ExternalSourceID equals source.Id
              where (homeTeam.TeamName == teamA && awayTeam.TeamName == teamB && EntityFunctions.TruncateTime(match.MatchDate) == matchDate.Date)
              let latestTimeStamp = probability.MatchOutcomeOdds.Max(m => m.TimeStamp)
              where takeAll || matchOdd.TimeStamp == latestTimeStamp
              select new OddsForEvent
              {
                Outcome = outcome.MatchOutcomeString,
                Probability = probability.MatchOutcomeProbability,
                OddsSource = source.Source,
                Bookmaker = bookmaker.BookmakerName,
                Odds = matchOdd.Odd,
                Edge = probability.MatchOutcomeProbability * matchOdd.Odd - 1,
                TimeStamp = matchOdd.TimeStamp,
                ClickThroughURL = new Uri(matchOdd.ClickThroughURL)
              };
      return matches;
    }

  }
}
