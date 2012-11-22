using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Value;

namespace Samurai.Domain.Value.Excel
{
  public class ExcelFixtureStrategyProvider : IFixtureStrategyProvider
  {
    private readonly ISpreadsheetData spreadsheetData;

    public ExcelFixtureStrategyProvider(ISpreadsheetData spreadsheetData)
    {
      if (spreadsheetData == null) throw new ArgumentNullException("spreadsheetData");
      this.spreadsheetData = spreadsheetData;
    }
    public IFixtureStrategy CreateFixtureStrategy(Model.SportEnum sport)
    {
      return new ExcelFixtureStrategy(spreadsheetData);
    }
  }
}
