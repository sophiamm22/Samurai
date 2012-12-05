using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Value;

namespace Samurai.Domain.Value.Excel
{
  public class ExcelPredictionStrategy : IPredictionStrategy
  {
    private readonly ISpreadsheetData spreadsheetData;

    public ExcelPredictionStrategy(ISpreadsheetData spreadsheetData)
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
