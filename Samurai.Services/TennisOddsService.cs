using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Tennis;
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
      IStoredProceduresRepository storedProcedureRepository, IPredictionRepository predictionRepository,
      ICouponStrategyProvider couponProvider, IOddsStrategyProvider oddsProvider)
      : base(fixtureRepository, bookmakerRepository, storedProcedureRepository, predictionRepository, 
      couponProvider, oddsProvider)
    {
      this.sport = "Tennis";
    }

    public IEnumerable<TennisCouponViewModel> FetchAllTennisOddsNew(DateTime date)
    {
      var matchCoupons = new List<TennisCouponViewModel>();

      var tournaments = DaysTournaments(date, this.sport);

      var oddsSources = this.bookmakerRepository.GetActiveOddsSources().Select(o => o.Source).ToList();

      foreach (var tournament in tournaments.Select(t => t.TournamentName))
      {
        foreach (var source in oddsSources)
        {
          matchCoupons.AddRange(FetchCouponsNew(date, tournament, source, this.sport, true, false));
        }
      }
      return matchCoupons;
    }

    public IEnumerable<TennisMatchViewModel> FetchAllTennisOdds(DateTime date)
    {
      var matchCoupons = new List<TennisMatchViewModel>();

      var tournaments = DaysTournaments(date, this.sport);

      var oddsSources = this.bookmakerRepository.GetActiveOddsSources().Select(o => o.Source).ToList();

      foreach (var tournament in tournaments.Select(t => t.TournamentName))
      {
        foreach (var source in oddsSources)
        {
          matchCoupons.AddRange(FetchCoupons(date, tournament, source, this.sport, true, false));
        }
      }
      return matchCoupons;
    }

    public IEnumerable<TennisCouponViewModel> FetchCouponsNew(DateTime date, string tournament, string oddsSource, string sport, bool getOdds, bool prescreen)
    {
      var coupons = FetchMatchCouponsNew(date, tournament, oddsSource, sport, getOdds, prescreen);
      return Mapper.Map<IEnumerable<Model.GenericMatchCoupon>, IEnumerable<TennisCouponViewModel>>(coupons);
    }

    public IEnumerable<TennisMatchViewModel> FetchCoupons(DateTime date, string tournament, string oddsSource, string sport, bool getOdds, bool prescreen)
    {
      var matches = FetchMatchCoupons(date, tournament, oddsSource, sport, getOdds, prescreen);
      return Mapper.Map<IEnumerable<Match>, IEnumerable<TennisMatchViewModel>>(matches);
    }

    protected override bool QualifiesPredicate(decimal probability, decimal odds, decimal edgeRequired, int gamesPlayed, int? minGamesRequired)
    {
      return base.QualifiesPredicate(probability, odds, edgeRequired, gamesPlayed, minGamesRequired) &&
        gamesPlayed >= (minGamesRequired ?? 0);
    }


  }
}
