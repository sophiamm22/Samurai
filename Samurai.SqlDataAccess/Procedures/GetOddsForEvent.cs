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
    public IEnumerable<OddsForEvent> GetAllOddsForEvent(DateTime matchDate, string oddsSource, string teamA, string teamB, string firstNameA = null, string firstNameB = null)
    {
      return GetOddsForEvent(matchDate, oddsSource, teamA, teamB, firstNameA, firstNameB, true).ToList();
    }

    public IEnumerable<OddsForEvent> GetLatestOddsForEvent(DateTime matchDate, string oddsSource, string teamA, string teamB, string firstNameA = null, string firstNameB = null)
    {
      return GetOddsForEvent(matchDate, oddsSource, teamA, teamB, firstNameA, firstNameB, false).ToList();
    }

    private IQueryable<OddsForEvent> GetOddsForEvent(DateTime matchDate, string oddsSource, string teamA, string teamB, string firstNameA, string firstNameB, bool takeAll)
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
              join matchCouponURL in DbSet<MatchCouponURL>() on match.Id equals matchCouponURL.MatchID

              where (homeTeam.Name == teamA &&
                     homeTeam.FirstName == firstNameA &&
                     awayTeam.Name == teamB &&
                     awayTeam.FirstName == firstNameB &&
                     EntityFunctions.TruncateTime(match.MatchDate) == matchDate.Date)

              let latestTimeStampForBookmaker =
                probability.MatchOutcomeOdds
                           .Where(x => x.Bookmaker == bookmaker && (x.ExternalSource.Source == oddsSource))
                           .Select(x => x.TimeStamp)
                           .DefaultIfEmpty(DateTime.MinValue)
                           .Max()

              where (takeAll && source.Source == oddsSource) || matchOdd.TimeStamp == latestTimeStampForBookmaker

              select new OddsForEvent
              {
                IsBetable = true,
                Outcome = outcome.MatchOutcomeString,
                OddBeforeCommission = matchOdd.Odd,
                CommissionPct = bookmaker.CurrentCommission.HasValue ? (double?)bookmaker.CurrentCommission : new Nullable<double>(),
                DecimalOdd = ((double)matchOdd.Odd) * (bookmaker.CurrentCommission == null ? 1.0 : (1.0 + (double)bookmaker.CurrentCommission.Value)), 
                TimeStamp = matchOdd.TimeStamp,
                Bookmaker = bookmaker.BookmakerName,
                OddsSource = source.Source,
                ClickThroughURL = matchOdd.ClickThroughURL,
                Priority = bookmaker.Priority,

                MatchCouponURL = matchCouponURL.MatchCouponURLString,
                BookmakerID = bookmaker.Id, 
                Edge = probability.MatchOutcomeProbability * matchOdd.Odd - 1,
                Probability = probability.MatchOutcomeProbability
              };
      return matches;
    }

  }
}
