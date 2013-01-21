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
    private readonly ISpreadsheetData spreadsheetData;

    public ExcelFootballFixtureStrategy(ISpreadsheetData spreadsheetData)
    {
      if (spreadsheetData == null) throw new ArgumentNullException("spreadsheetData");
      this.spreadsheetData = spreadsheetData;
    }

    public IEnumerable<GenericMatchDetailQuery> UpdateFixtures(DateTime fixtureDate)
    {
      throw new NotImplementedException();
      //return UpdateResults(fixtureDate);
    }

    public IEnumerable<GenericMatchDetailQuery> UpdateResults(DateTime fixtureDate)
    {
      throw new NotImplementedException();
      //return this.spreadsheetData.UpdateResults(fixtureDate);
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
