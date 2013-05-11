using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Domain.Entities;

namespace Samurai.Domain.Repository
{
  public interface IPredictionRepositorydsfsdf
  {
    Uri GetFootballAPIURL(int teamAID, int teamBID);
    Uri GetTodaysMatchesURL();
    IEnumerable<Match> GetDaysFootballMatches(Competition competition, DateTime date);
    string GetPredictionCompetitionAlias(string predictionName, ExternalSource source);
    int GetGamesRequiredForBet();
    double GetOverroundRequired(Sport sport);
    Fund GetFundDetails(string fundName);
  }
}
