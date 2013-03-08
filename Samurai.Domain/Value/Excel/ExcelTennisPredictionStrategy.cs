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
    private IFootballSpreadsheetData spreadsheetData;

    public ExcelTennisPredictionStrategy(IFootballSpreadsheetData spreadsheetData)
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


    public Model.GenericPrediction FetchSinglePrediction(TeamPlayer teamPlayerA, TeamPlayer teamPlayerB, Tournament tournament, Model.IValueOptions valueOptions)
    {
      throw new NotImplementedException();
    }
  }
}
