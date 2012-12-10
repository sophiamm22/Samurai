using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Entities;

namespace Samurai.Domain.Value.Excel
{
  public class ExcelTennisFixtureStrategy : IFixtureStrategy
  {
    private readonly ISpreadsheetData spreadsheetData;

    public ExcelTennisFixtureStrategy(ISpreadsheetData spreadsheetData)
    {
      if (spreadsheetData == null) throw new ArgumentNullException("spreadsheetData");
      this.spreadsheetData = spreadsheetData;
    }

    public IEnumerable<Match> UpdateFixtures(DateTime fixtureDate)
    {
      return UpdateResults(fixtureDate);
    }

    public IEnumerable<Match> UpdateResults(DateTime fixtureDate, string reusedHTML = "")
    {
      return this.spreadsheetData.UpdateResults(fixtureDate);
    }
  }
}
