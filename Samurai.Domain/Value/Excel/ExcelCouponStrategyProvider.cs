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
    private readonly IFootballSpreadsheetData footballSpreadsheetData;
    private readonly ITennisSpreadsheetData tennisSpreadsheetData;

    public ExcelCouponStrategyProvider(IFootballSpreadsheetData footballSpreadsheetData,
      ITennisSpreadsheetData tennisSpreadsheetData)
    {
      if (footballSpreadsheetData == null) throw new ArgumentNullException("footballSpreadsheetData");
      if (tennisSpreadsheetData == null) throw new ArgumentNullException("tennisSpreadsheetData");

      this.footballSpreadsheetData = footballSpreadsheetData;
      this.tennisSpreadsheetData = tennisSpreadsheetData;
    }

    public ICouponStrategy CreateCouponStrategy(IValueOptions valueOptions)
    {
      if (valueOptions.Sport.SportName == "Football")
        return new ExcelFootballCouponStrategy(this.footballSpreadsheetData);
      else if (valueOptions.Sport.SportName == "Tennis")
        return new ExcelTennisCouponStrategy(this.tennisSpreadsheetData);
      else
        throw new ArgumentException("valueOptions.Sport.SportName");
    }
  }
}
