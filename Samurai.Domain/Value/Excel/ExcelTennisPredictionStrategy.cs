using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Value;
using Samurai.Domain.Repository;
using Model = Samurai.Domain.Model;
using Samurai.Domain.Entities;

namespace Samurai.Domain.Value.Excel
{
  public class ExcelTennisPredictionStrategy : IPredictionStrategy
  {
    private ISpreadsheetData spreadsheetData;

    public ExcelTennisPredictionStrategy(ISpreadsheetData spreadsheetData)
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
