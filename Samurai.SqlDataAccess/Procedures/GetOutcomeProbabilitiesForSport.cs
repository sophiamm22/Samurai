using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Objects;

using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;

namespace Samurai.SqlDataAccess.Procedures
{
  public partial class SqlStoredProceduresRepository
  {
    public IEnumerable<OutcomeProbabilitiesForSport> GetOutcomeProbabilitiesForSport(DateTime matchDate, string sportName)
    {
      var outcomeProbs = (
        from match in DbSet<Match>()
        join tennisStat in DbSet<TennisPredictionStat>() on match.Id equals tennisStat.Id into joinedTennisStat
        from tennisStat in joinedTennisStat.DefaultIfEmpty()
        join homeTeam in DbSet<TeamPlayer>() on match.TeamAID equals homeTeam.Id
        join awayTeam in DbSet<TeamPlayer>() on match.TeamBID equals awayTeam.Id
        join tournamentEvent in DbSet<TournamentEvent>() on match.TournamentEventID equals tournamentEvent.Id
        join tournament in DbSet<Tournament>() on tournamentEvent.TournamentID equals tournament.Id
        join competition in DbSet<Competition>() on tournament.CompetitionID equals competition.Id
        join sport in DbSet<Sport>() on competition.SportID equals sport.Id


        where EntityFunctions.TruncateTime(match.MatchDate) == matchDate.Date && sport.SportName == sportName
        select new OutcomeProbabilitiesForSport
        {
          MatchID = match.Id,
          Tournament = tournament.TournamentName,
          Date = match.MatchDate,
          HomeTeam = homeTeam.Slug,
          AwayTeam = awayTeam.Slug,
          EdgeRequired = competition.EdgeRequired,
          GamesRequiredForBet = competition.GamesRequiredForBet,
          GamesPlayedA = tennisStat == null ? new Nullable<int>() : tennisStat.PlayerAGames,
          GamesPlayedB = tennisStat == null ? new Nullable<int>() : tennisStat.PlayerBGames,
        })
        .ToList();

      outcomeProbs
        .ForEach(match =>
          {
            match.OutcomeProbabilties = DbSet<MatchOutcomeProbabilitiesInMatch>()
                                          .Where(m => m.MatchID == match.MatchID)
                                          .ToDictionary(o => o.MatchOutcomeID, o => o.MatchOutcomeProbability);
          });

      return outcomeProbs;
    }
  }
}
