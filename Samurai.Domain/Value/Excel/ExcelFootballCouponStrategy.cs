﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Domain.Model;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Core;
using Samurai.Domain.HtmlElements;

namespace Samurai.Domain.Value.Excel
{
  public class ExcelFootballCouponStrategy : ICouponStrategy
  {
    private readonly IFootballSpreadsheetData spreadsheetData;

    public ExcelFootballCouponStrategy(IFootballSpreadsheetData spreadsheetData)
    {
      if (spreadsheetData == null) throw new ArgumentNullException("spreadsheetData");
      this.spreadsheetData = spreadsheetData;
    }

    public IEnumerable<IGenericTournamentCoupon> GetTournaments(OddsDownloadStage stage = OddsDownloadStage.Tournament)
    {
      return this.spreadsheetData.GetTournaments(stage);
    }

    public IEnumerable<GenericMatchCoupon> GetMatches(Uri tournamentURL)
    {
      return this.spreadsheetData.GetMatches(tournamentURL);
    }

    public IEnumerable<GenericMatchCoupon> GetMatches()
    {
      return this.spreadsheetData.GetMatches();
    }
  }
}
