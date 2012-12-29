﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Contracts
{
  public interface IPredictionRepository
  {
    void AddOrUpdateTennisPredictionsStats(TennisPredictionStat stat);
    Uri GetFootballAPIURL(int teamAID, int teamBID);
    Uri GetTodaysMatchesURL();
    string GetTournamentAlias(string tournamentName, ExternalSource source);
    int GetGamesRequiredForBet(string competitionName);
    decimal GetOverroundRequired(string competitionName);
    Fund GetFundDetails(string fundName);
    void SaveChanges();
    IQueryable<MatchOutcomeProbabilitiesInMatch> GetMatchOutcomeProbabilities(int matchID);
    void AddMatchOutcomeProbabilitiesInMatch(MatchOutcomeProbabilitiesInMatch entity);
    IQueryable<ScoreOutcomeProbabilitiesInMatch> GetScoreOutcomeProbabilities(int matchID);
    void AddScoreOutcomeProbabilities(ScoreOutcomeProbabilitiesInMatch entity);
    IQueryable<MatchOutcomeProbabilitiesInMatch> GetMatchOutcomeProbabiltiesInMatchByDate(DateTime fixtureDate, string sport);
    IDictionary<int, IEnumerable<ScoreOutcomeProbabilitiesInMatch>> GetScoreOutcomeProbabilitiesInMatchByIDs(IEnumerable<int> ids);
    IDictionary<int, IEnumerable<MatchOutcomeProbabilitiesInMatch>> GetMatchOutcomeProbabilitiesInMatchByIDs(IEnumerable<int> ids);
  }
}
