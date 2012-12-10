using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Value;

namespace Samurai.Domain.Value.Excel
{
  public class ExcelFootballPredictionStrategy : IPredictionStrategy
  {
    private readonly ISpreadsheetData spreadsheetData;

    public ExcelFootballPredictionStrategy(ISpreadsheetData spreadsheetData)
    {
      if (spreadsheetData == null) throw new ArgumentNullException("spreadsheetData");
      this.spreadsheetData = spreadsheetData;
    }

    public IEnumerable<Model.GenericPrediction> GetPredictions(Model.IValueOptions valueOptions)
    {
      return this.spreadsheetData.GetPredictions(valueOptions);
    }
  }
}
