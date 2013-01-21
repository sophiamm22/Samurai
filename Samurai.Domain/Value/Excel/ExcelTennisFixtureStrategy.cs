﻿using System;
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
    private readonly ISpreadsheetData spreadsheetData;

    public ExcelTennisFixtureStrategy(ISpreadsheetData spreadsheetData)
    {
      if (spreadsheetData == null) throw new ArgumentNullException("spreadsheetData");
      this.spreadsheetData = spreadsheetData;
    }

    public IEnumerable<GenericMatchDetailQuery> UpdateFixturesNew(DateTime fixtureDate)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<Match> UpdateFixtures(DateTime fixtureDate)
    {
      return UpdateResults(fixtureDate);
    }

    public IEnumerable<GenericMatchDetailQuery> UpdateResultsNew(DateTime fixtureDate)
    {
      throw new NotImplementedException();
    }    
    
    public IEnumerable<Match> UpdateResults(DateTime fixtureDate, string reusedHTML = "")
    {
      return this.spreadsheetData.UpdateResults(fixtureDate);
    }

    public IEnumerable<TournamentEvent> UpdateTournamentEvents()
    {
      throw new NotImplementedException();
    }



    public APITournamentDetail GetTournamentDetail(string tournament, int year)
    {
      throw new NotImplementedException();
    }
  }
}
