using System;
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
  public class ExcelTennisCouponStrategy : ICouponStrategy
  {
    private readonly ISpreadsheetData spreadsheetData;

    public ExcelTennisCouponStrategy(ISpreadsheetData spreadsheetData)
    {
      if (spreadsheetData == null) throw new ArgumentNullException("spreadsheetData");
      this.spreadsheetData = spreadsheetData;
    }

    public IEnumerable<Model.IGenericTournamentCoupon> GetTournaments(Model.OddsDownloadStage stage = OddsDownloadStage.Tournament)
    {
      return this.spreadsheetData.GetTournaments(stage);
    }

    public IEnumerable<Model.IGenericMatchCoupon> GetMatches(Uri tournamentURL)
    {
      return this.spreadsheetData.GetMatches(tournamentURL);
    }

    public IEnumerable<Model.IGenericMatchCoupon> GetMatches()
    {
      return this.spreadsheetData.GetMatches();
    }
  }
}
