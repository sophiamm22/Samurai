using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

using Infrastructure.Data;

namespace Samurai.SqlDataAccess.SQLProcedures
{
  public class DaysBestOddsForSportArgs : IStoredProc
  {
    private const string storedProcName = "sp_Get_Days_Best_Odds_For_Sport";

    [NotMapped]
    public string StoredProcName { get { return storedProcName; } }

    [StoredProcAttributes.Name("sport")]
    [StoredProcAttributes.ParameterType(SqlDbType.NVarChar)]
    public string Sport { get; set; }

    [StoredProcAttributes.Name("date")]
    [StoredProcAttributes.ParameterType(SqlDbType.Date)]
    public DateTime OddsDate { get; set; }
  }
}
