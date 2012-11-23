using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Domain.Value;
using Model = Samurai.Domain.Model;

namespace Samurai.Services
{
  public class TennisOddsService : OddsService, ITennisOddsService
  {
    public TennisOddsService(IFixtureRepository fixtureRepository, IBookmakerRepository bookmakerRepository,
      IStoredProceduresRepository storedProcedureRepository,
      ICouponStrategyProvider couponProvider, IOddsStrategyProvider oddsProvider)
      : base(fixtureRepository, bookmakerRepository, storedProcedureRepository,
      couponProvider, oddsProvider)
    {
      this.sport = "Tennis";
    }

    public IEnumerable<TennisMatchViewModel> FetchAllTennisOdds(DateTime date)
    {
      var matchCoupons = new List<TennisMatchViewModel>();

      var tournaments = DaysTournaments(date, this.sport);
      var oddsSources = this.bookmakerRepository.GetActiveOddsSources().Select(o => o.Source);

      foreach (var tournament in tournaments)
      {
        foreach (var source in oddsSources)
        {
          //matchCoupons.AddRange(
        }
      }
      return matchCoupons;
    }
  }
}
