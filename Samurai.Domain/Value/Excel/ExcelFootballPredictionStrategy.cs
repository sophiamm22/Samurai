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
    private readonly IFootballSpreadsheetData spreadsheetData;

    public ExcelFootballPredictionStrategy(IFootballSpreadsheetData spreadsheetData)
    {
      if (spreadsheetData == null) throw new ArgumentNullException("spreadsheetData");
      this.spreadsheetData = spreadsheetData;
    }

    public IEnumerable<Model.GenericPrediction> FetchPredictions(Model.IValueOptions valueOptions)
    {
      return this.spreadsheetData.GetPredictions(valueOptions);
    }


    public IEnumerable<Model.GenericPrediction> FetchPredictionsCoupon(Model.IValueOptions valueOptions)
    {
      return this.spreadsheetData.GetPredictions(valueOptions);
    }


    public Model.GenericPrediction FetchSinglePrediction(Entities.TeamPlayer teamPlayerA, Entities.TeamPlayer teamPlayerB, Entities.Tournament tournament, Model.IValueOptions valueOptions)
    {
      throw new NotImplementedException();
    }
  }
}
