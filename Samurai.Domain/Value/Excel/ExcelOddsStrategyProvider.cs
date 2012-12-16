using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Value;

namespace Samurai.Domain.Value.Excel
{
  public class ExcelOddsStrategyProvider : IOddsStrategyProvider
  {
    private readonly ISpreadsheetData spreadsheetData;

    public ExcelOddsStrategyProvider(ISpreadsheetData spreadsheetData)
    {
      if (spreadsheetData == null) throw new ArgumentNullException("spreadsheetData");
      this.spreadsheetData = spreadsheetData;
    }

    public IOddsStrategy CreateOddsStrategy(Model.IValueOptions valueOptions)
    {
      if (valueOptions.Sport.SportName == "Football")
        return new ExcelFootballOddsStrategy(this.spreadsheetData);
      else if (valueOptions.Sport.SportName == "Tennis")
        return new ExcelTennisOddsStrategy(this.spreadsheetData);
      else
        throw new ArgumentException("valueOptions.Sport.SportName");
    }
  }
}
