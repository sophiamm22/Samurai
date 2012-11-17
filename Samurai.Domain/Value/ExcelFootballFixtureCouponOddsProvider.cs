using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model = Samurai.Domain.Model;
using Samurai.Domain.Entities;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.Domain.Value
{
  //public interface IExcelFootballFixtureCouponOddsProvider : IFixtureStrategyProvider, ICouponStrategyProvider, IOddsStrategyProvider
  //{
  //}

  public class ExcelFootballFixtureCouponOddsProvider //: IExcelFootballFixtureCouponOddsProvider
  {
    private static IFixturesAndOdds excelFootballFixtureCouponOddsStrategy;

    public ExcelFootballFixtureCouponOddsProvider(IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IPredictionRepository predictionRepository,
      Model.IValueOptions valueOptions)
    {
      if (bookmakerRepository == null) throw new ArgumentNullException("bookmakerRepository");
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (predictionRepository == null) throw new ArgumentNullException("predictionRepository");
      if (valueOptions == null) throw new ArgumentNullException("valueOptions");

      //if (excelFootballFixtureCouponOddsStrategy == null)
      //  excelFootballFixtureCouponOddsStrategy = new ExcelFootballFixtureCouponOddsStrategy(
      //    bookmakerRepository, fixtureRepository, predictionRepository, valueOptions);
    }

    public IFixtureStrategy CreateFixtureStrategy(Model.SportEnum sport)
    {
      return excelFootballFixtureCouponOddsStrategy;
    }

    public ICouponStrategy CreateCouponStrategy(Model.IValueOptions valueOptions)
    {
      return excelFootballFixtureCouponOddsStrategy;
    }

    public IOddsStrategy CreateOddsStrategy(Model.IValueOptions valueOptions)
    {
      return excelFootballFixtureCouponOddsStrategy;
    }
  }
}
