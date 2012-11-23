using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Contracts
{
  public interface IPredictionRepository
  {
    void AddOrUpdateTennisPredictionsStats(int matchID, TennisPredictionStat stat);
    Uri GetFootballAPIURL(int teamAID, int teamBID);
    Uri GetTodaysMatchesURL();
    string GetTournamentAlias(string tournamentName, ExternalSource source);
    int GetGamesRequiredForBet(string competitionName);
    decimal GetOverroundRequired(string competitionName);
    Fund GetFundDetails(string fundName);
    
  }
}
