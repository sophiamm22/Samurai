using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Entities;

namespace Samurai.Domain.Value.Excel
{
  public class ExcelFootballFixtureStrategy : IFixtureStrategy
  {
    private readonly ISpreadsheetData spreadsheetData;

    public ExcelFootballFixtureStrategy(ISpreadsheetData spreadsheetData)
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

    public IEnumerable<Entities.ComplexTypes.GenericMatchDetailQuery> UpdateFixturesNew(DateTime fixtureDate)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<Entities.ComplexTypes.GenericMatchDetailQuery> UpdateResultsNew(DateTime fixtureDate)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<TournamentEvent> UpdateTournamentEvents()
    {
      throw new NotImplementedException();
    }
  }
}
