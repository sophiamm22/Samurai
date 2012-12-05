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

  }
}
