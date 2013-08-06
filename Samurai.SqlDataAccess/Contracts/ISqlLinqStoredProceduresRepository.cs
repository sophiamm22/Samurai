﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;

namespace Samurai.SqlDataAccess.Contracts
{
  public interface ISqlLinqStoredProceduresRepository
  {
    IEnumerable<OddsForEvent> GetBestOddsFromMatchID(int matchID, string oddsSource);
    IEnumerable<OddsForEvent> GetAllOddsForEvent(DateTime matchDate, string oddsSource, string teamA, string teamB, string firstNameA = null, string firstNameB = null);
    IEnumerable<OddsForEvent> GetLatestOddsForEvent(DateTime matchDate, string oddsSource, string teamA, string teamB, string firstNameA = null, string firstNameB = null);
    IEnumerable<OddsForEvent> GetAllOddsForPeriod(DateTime startDate, DateTime endDate, string oddsSource);
    IEnumerable<OddsForEvent> GetLatestOddsForPeriod(DateTime startDate, DateTime endDate, string oddsSource);
    IEnumerable<OddsForEvent> GetAllOddsFromMatchID(int matchID, string oddsSource);
    IEnumerable<OddsForEvent> GetLatestOddsFromMatchID(int matchID, string oddsSource);
    IEnumerable<OutcomeProbabilitiesForSport> GetOutcomeProbabilitiesForSport(DateTime matchDate, string sportName);
    IQueryable<GenericMatchDetailQuery> GetGenericMatchDetails(DateTime matchDate, string queriedSport);
  }
}