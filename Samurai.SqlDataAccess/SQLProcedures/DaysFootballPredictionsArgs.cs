using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

using Infrastructure.Data;

namespace Samurai.SqlDataAccess.SQLProcedures
{
  public class DaysFootballPredictionsArgs : IStoredProc
  {
    private const string storedProcName = "sp_Get_Days_Football_Predictions";

    [NotMapped]
    public string StoredProcName { get { return storedProcName; } }

    [StoredProcAttributes.Name("date")]
    [StoredProcAttributes.ParameterType(SqlDbType.Date)]
    public DateTime PredictionDate { get; set; }
  }
}
