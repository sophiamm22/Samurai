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
  public class SqlPredictionRepository : GenericRepository, IPredictionRepository
  {
    public SqlPredictionRepository(DbContext context)
      :base(context)
    { }

    public void AddOrUpdateTennisPredictionsStats(TennisPredictionStat stat)
    {
      var persistedStat = First<TennisPredictionStat>(s => s.Id == stat.Id);
      if (persistedStat == null)
      {
        Add<TennisPredictionStat>(stat);
      }
      else
      {
        persistedStat.PlayerAGames = stat.PlayerAGames;
        persistedStat.PlayerBGames = stat.PlayerBGames;
        persistedStat.EPoints = stat.EPoints;
        persistedStat.EGames = stat.EGames;
        persistedStat.ESets = stat.ESets;
      }
    }

    public Uri GetFootballAPIURL(int teamAID, int teamBID)
    {
      return new Uri(string.Format("http://www.dectech.org/cgi-bin/new_site/GetEuroIntlSimulatedFast.pl?homeID={0}&awayType=0&awayID={1}&homeType=0&neutral=0",
        teamAID, teamBID));
    }

    public Uri GetTodaysMatchesURL()
    {
      return new Uri("http://www.tennisbetting365.com/api/gettodaysmatches");
    }

    public Uri GetTennisPredictionURL(TeamPlayer playerA, TeamPlayer playerB, Tournament tournament, DateTime date)
    {
      return new Uri(
        string.Format("http://www.tennisbetting365.com/api/getprediction/{0}/{1}/{2}/{3}/vs/{4}/{5}",
        tournament.Slug,
        date.Year,
        playerA.FirstName.RemoveDiacritics().ToLower().Replace(' ','-'),
        playerA.Name.RemoveDiacritics().ToLower().Replace(' ', '-'),
        playerB.FirstName.RemoveDiacritics().ToLower().Replace(' ', '-'),
        playerB.Name.RemoveDiacritics().ToLower().Replace(' ', '-'))
        );

    }

    public string GetTournamentAlias(string tournamentName, ExternalSource externalSource)
    {
      var tournamentAlias = GetQuery<TournamentExternalSourceAlias>()
                              .Include(m => m.Tournament)
                              .Where(a => a.Alias == tournamentName &&
                                          a.ExternalSource.Source == externalSource.Source);

      if (tournamentAlias.Count() == 0)
        return null;
      else
        return tournamentAlias.First().Tournament.TournamentName;
    }

    public int GetGamesRequiredForBet(string competitionName)
    {
      var comp = First<Competition>(c => c.CompetitionName == competitionName);
      return comp == null ? 0 : (comp.GamesRequiredForBet ?? 0);
    }

    public decimal GetOverroundRequired(string competitionName)
    {
      var comp = First<Competition>(c => c.CompetitionName == competitionName);
      return comp == null ? 0 : comp.EdgeRequired;
    }

    public Fund GetFundDetails(string fundName)
    {
      return First<Fund>(f => f.FundName == fundName);
    }

    public void SaveChanges()
    {
      UnitOfWork.SaveChanges();
    }

    public IQueryable<MatchOutcomeProbabilitiesInMatch> GetMatchOutcomeProbabilities(int matchID)
    {
      return GetQuery<MatchOutcomeProbabilitiesInMatch>(o => o.MatchID == matchID);
    }

    public void AddMatchOutcomeProbabilitiesInMatch(MatchOutcomeProbabilitiesInMatch entity)
    {
      Add<MatchOutcomeProbabilitiesInMatch>(entity);
    }

    public IQueryable<ScoreOutcomeProbabilitiesInMatch> GetScoreOutcomeProbabilities(int matchID)
    {
      return GetQuery<ScoreOutcomeProbabilitiesInMatch>(o => o.MatchID == matchID);
    }

    public void AddScoreOutcomeProbabilities(ScoreOutcomeProbabilitiesInMatch entity)
    {
      Add<ScoreOutcomeProbabilitiesInMatch>(entity);
    }

    public IQueryable<MatchOutcomeProbabilitiesInMatch> GetMatchOutcomeProbabiltiesInMatchByDate(DateTime fixtureDate, string sport)
    {
      return GetQuery<MatchOutcomeProbabilitiesInMatch>(m => EntityFunctions.TruncateTime(m.Match.MatchDate) == fixtureDate.Date &&
                                                             m.Match.TournamentEvent.Tournament.Competition.Sport.SportName == sport);
    }

    public IDictionary<int, List<ScoreOutcomeProbabilitiesInMatch>> GetScoreOutcomeProbabilitiesInMatchByIDs(IEnumerable<int> ids)
    {
      var selection = (from scoreProbs in GetQuery<ScoreOutcomeProbabilitiesInMatch>()
                                          .Include(s => s.ScoreOutcome)
                      where ids.Contains(scoreProbs.MatchID)
                      group scoreProbs by scoreProbs.MatchID into groupedScoreProbs
                      select new
                      {
                        ID = groupedScoreProbs.Key,
                        Probs = groupedScoreProbs
                      }).ToList();
      return selection.ToDictionary(s => s.ID, s => s.Probs.ToList());
    }

    public IDictionary<int, List<MatchOutcomeProbabilitiesInMatch>> GetMatchOutcomeProbabilitiesInMatchByIDs(IEnumerable<int> ids)
    {
      var selection = (from matchProbs in GetQuery<MatchOutcomeProbabilitiesInMatch>()
                      where ids.Contains(matchProbs.MatchID)
                      group matchProbs by matchProbs.MatchID into groupedMatchProbs

                      select new
                      {
                        ID = groupedMatchProbs.Key,
                        Probs = groupedMatchProbs
                      }).ToList();
      return selection.ToDictionary(s => s.ID, s => s.Probs.ToList());
    }

    public IQueryable<TennisPredictionStat> GetTennisPredictionStatByMatchIDs(IEnumerable<int> ids)
    {
      var selection = from predictionStat in GetQuery<TennisPredictionStat>()
                      where ids.Contains(predictionStat.Id)
                      select predictionStat;
      return selection;
    }

  }
}
