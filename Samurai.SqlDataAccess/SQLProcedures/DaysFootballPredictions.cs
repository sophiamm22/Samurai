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
  public class DaysFootballPredictionsParams : IStoredProc
  {
    private const string storedProcName = "sp_Get_Days_Football_Predictions";

    [NotMapped]
    public string StoredProcName { get { return storedProcName; } }

    [StoredProcAttributes.Name("date")]
    [StoredProcAttributes.ParameterType(SqlDbType.Date)]
    public DateTime PredictionDate { get; set; }
  }

  public class DaysFootballPredictions
  {
    public int MatchID_pk { get; set; }
    public string TournamentName { get; set; }
    public int Year { get; set; }
    public DateTime MatchDate { get; set; }
    public string TeamA { get; set; }
    public string TeamB { get; set; }
    public decimal TeamAProbability { get; set; }
    public decimal Draw { get; set; }
    public decimal TeamBProbability { get; set; }
    public string Score { get; set; }
  }
}
