using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Value;

namespace Samurai.Domain.Value.Excel
{
  public class ExcelTennisOddsStrategy : IOddsStrategy
  {
    private readonly ISpreadsheetData spreadsheetData;

    public ExcelTennisOddsStrategy(ISpreadsheetData spreadsheetData)
    {
      if (spreadsheetData == null) throw new ArgumentNullException("spreadsheetData");
      this.spreadsheetData = spreadsheetData;
    }

    public IDictionary<Model.Outcome, IEnumerable<Model.GenericOdd>> GetOdds(Model.IGenericMatchCoupon matchCoupon, DateTime timeStamp)
    {
      return this.spreadsheetData.GetOdds(matchCoupon, timeStamp);
    }
  }
}
