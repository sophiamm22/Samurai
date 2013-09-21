using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Entities.ComplexTypes;

namespace Samurai.SqlDataAccess.Contracts
{
  public interface ISqlStoredProceduresRepository
  {
    IEnumerable<DaysTennisPredictions> GetDaysTennisPredictions(DateTime predictionDate);
    IEnumerable<DaysFootballPredictions> GetDaysFootballPredictions(DateTime predictionsDate);
    IEnumerable<DaysBestOddsForSport> GetDaysBestOddsForSport(DateTime oddsDate, string sport);
  }
}
