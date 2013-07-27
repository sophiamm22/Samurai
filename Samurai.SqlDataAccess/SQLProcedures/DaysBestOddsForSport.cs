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
  public class DaysBestOddsForSportParams : IStoredProc
  {
    private const string storedProcName = "sp_Get_Days_Best_Odds_For_Sport";

    [NotMapped]
    public string StoredProcName { get { return storedProcName; } }

    [StoredProcAttributes.Name("sport")]
    [StoredProcAttributes.ParameterType(SqlDbType.NVarChar)]
    public DateTime Sport { get; set; }

    [StoredProcAttributes.Name("date")]
    [StoredProcAttributes.ParameterType(SqlDbType.Date)]
    public DateTime OddsDate { get; set; }
  }

  public class DaysBestOddsForSport
  {
    public int MatchID_pk { get; set; }
    public string SportName { get; set; }
    public string MatchOutcome { get; set; }
    public decimal Odd { get; set; }
    public decimal? CurrentCommission { get; set; }
    public decimal OddAfterCommission { get; set; }
    public string BookmakerName { get; set; }
    public string Source { get; set; }
    public string ClickThroughURL { get; set; }
  }

}
