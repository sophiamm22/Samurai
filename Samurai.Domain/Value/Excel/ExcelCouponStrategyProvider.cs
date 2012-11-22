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
      return new ExcelCouponStrategy(this.spreadsheetData);
    }
  }
}
