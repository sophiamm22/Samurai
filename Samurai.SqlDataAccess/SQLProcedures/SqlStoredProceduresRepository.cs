using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Objects;

using Infrastructure.Data;

using Samurai.Domain.Entities;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Entities.ComplexTypes;

namespace Samurai.SqlDataAccess.SQLProcedures
{
  public class SqlStoredProceduresRepository : GenericRepository, ISqlStoredProceduresRepository
  {
    public SqlStoredProceduresRepository(DbContext context)
      : base(context)
    { }

    public IEnumerable<DaysTennisPredictions> GetDaysTennisPredictions(DateTime predictionDate)
    {
      var args = new DaysTennisPredictionsArgs()
      {
        PredictionDate = predictionDate
      };
      return
        base.ExecuteStoredProc<DaysTennisPredictions, DaysTennisPredictionsArgs>(args)
            .ToList();
    }

    public IEnumerable<DaysFootballPredictions> GetDaysFootballPredictions(DateTime predictionsDate)
    {
      var args = new DaysFootballPredictionsArgs()
      {
        PredictionDate = predictionsDate
      };
      return
        base.ExecuteStoredProc<DaysFootballPredictions, DaysFootballPredictionsArgs>(args)
            .ToList();
    }

    public IEnumerable<DaysBestOddsForSport> GetDaysBestOddsForSport(DateTime oddsDate, string sport)
    {
      var args = new DaysBestOddsForSportArgs()
      {
        OddsDate = oddsDate,
        Sport = sport
      };
      return
        base.ExecuteStoredProc<DaysBestOddsForSport, DaysBestOddsForSportArgs>(args)
            .ToList();
    }

  }
}
