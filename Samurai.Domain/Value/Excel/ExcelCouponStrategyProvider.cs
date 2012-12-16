using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Domain.Model;
using Samurai.Domain.Repository;
using Samurai.Domain.HtmlElements;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.Domain.Value.Excel
{
  public class ExcelCouponStrategyProvider : ICouponStrategyProvider
  {
    private readonly ISpreadsheetData spreadsheetData;

    public ExcelCouponStrategyProvider(ISpreadsheetData spreadsheetData)
    {
      if (spreadsheetData == null) throw new ArgumentNullException("spreadsheetData");
      this.spreadsheetData = spreadsheetData;
    }

    public ICouponStrategy CreateCouponStrategy(IValueOptions valueOptions)
    {
      if (valueOptions.Sport.SportName == "Football")
        return new ExcelFootballCouponStrategy(this.spreadsheetData);
      else if (valueOptions.Sport.SportName == "Tennis")
        return new ExcelTennisCouponStrategy(this.spreadsheetData);
      else
        throw new ArgumentException("valueOptions.Sport.SportName");
    }
  }
}
