using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;

namespace Samurai.SqlDataAccess.Contracts
{
  public interface IStoredProceduresRepository
  {
    IEnumerable<OddsForEvent> GetAllOddsForEvent(DateTime matchDate, string oddsSource, string teamA, string teamB, string firstNameA = null, string firstNameB = null);
    IEnumerable<OddsForEvent> GetLatestOddsForEvent(DateTime matchDate, string oddsSource, string teamA, string teamB, string firstNameA = null, string firstNameB = null);
    IEnumerable<OutcomeProbabilitiesForSport> GetOutcomeProbabilitiesForSport(DateTime matchDate, string sportName);
    IQueryable<GenericMatchDetailQuery> GetGenericMatchDetails(DateTime matchDate, string queriedSport);
  }
}
