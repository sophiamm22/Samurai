using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Value;

namespace Samurai.Domain.Value.Excel
{
  public class ExcelPredictionStrategyProvider : IPredictionStrategyProvider
  {
    private readonly ISpreadsheetData spreadsheetData;

    public ExcelPredictionStrategyProvider(ISpreadsheetData spreadsheetData)
    {
      if (spreadsheetData == null) throw new ArgumentNullException("spreadsheetData");
      this.spreadsheetData = spreadsheetData;
    }

    public IPredictionStrategy CreatePredictionStrategy(Entities.Sport sport)
    {
      return new ExcelFootballPredictionStrategy(this.spreadsheetData);
    }
  }
}
