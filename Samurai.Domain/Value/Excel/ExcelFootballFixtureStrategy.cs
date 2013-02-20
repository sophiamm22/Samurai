using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;

namespace Samurai.Domain.Value.Excel
{
  public class ExcelFootballFixtureStrategy : IFootballFixtureStrategy
  {
    private readonly IFootballSpreadsheetData spreadsheetData;

    public ExcelFootballFixtureStrategy(IFootballSpreadsheetData spreadsheetData)
    {
      if (spreadsheetData == null) throw new ArgumentNullException("spreadsheetData");
      this.spreadsheetData = spreadsheetData;
    }

    public IEnumerable<GenericMatchDetailQuery> UpdateFixtures(DateTime fixtureDate)
    {
      return UpdateResults(fixtureDate);
    }

    public IEnumerable<GenericMatchDetailQuery> UpdateResults(DateTime fixtureDate)
    {
      return this.spreadsheetData.UpdateResults(fixtureDate);
    }

    public IEnumerable<TournamentEvent> UpdateTournamentEvents()
    {
      throw new NotImplementedException();
    }
  }
}
