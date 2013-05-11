using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Domain.APIModel;

namespace Samurai.Domain.Value.Excel
{
  public class ExcelTennisFixtureStrategy : ITennisFixtureStrategy
  {
    private readonly ITennisSpreadsheetData spreadsheetData;

    public ExcelTennisFixtureStrategy(ITennisSpreadsheetData spreadsheetData)
    {
      if (spreadsheetData == null) throw new ArgumentNullException("spreadsheetData");
      this.spreadsheetData = spreadsheetData;
    }

    public IEnumerable<TournamentEvent> UpdateTournamentEvents()
    {
      throw new NotImplementedException();
    }

    public IEnumerable<GenericMatchDetailQuery> UpdateResults(DateTime fixtureDate)
    {
      return this.spreadsheetData.UpdateResults(fixtureDate);
    }

    public APITournamentDetail GetTournamentDetail(string tournament, int year)
    {
      throw new NotImplementedException();
    }
  }
}
