using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Tennis;
using Samurai.Web.ViewModels.Value;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Domain.Value;
using Samurai.Domain.Exceptions;
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

    public IEnumerable<OddViewModel> GetSingleTennisOdds(DateTime date, TennisFixtureViewModel fixture)
    {
      var oddsSources =
        this.bookmakerRepository
            .GetActiveOddsSources()
            .ToList();

      var allOdds = new List<OddsForEvent>();
      var playerAOdds = new List<OddsForEvent>();
      var playerBOdds = new List<OddsForEvent>();

      foreach (var oddsSource in oddsSources)
      {
        var oddsForEvent =
          this.storedProcedureRepository
              .GetLatestOddsForEvent(date,
                                     oddsSource.Source,
                                     fixture.PlayerASurname,
                                     fixture.PlayerBSurname,
                                     fixture.PlayerAFirstName,
                                     fixture.PlayerBFirstName)
              .ToList();

        playerAOdds.AddRange(oddsForEvent.Where(x => x.Outcome == "Home Win"));
        playerBOdds.AddRange(oddsForEvent.Where(x => x.Outcome == "Away Win"));
      }

      allOdds.AddRange(playerAOdds.Where(x => x.DecimalOdd == playerAOdds.Max(m => m.DecimalOdd)).OrderBy(x => (50 - x.OddsSource.Length) + ((x.OddsSource.Length % 2) * 10)).Take(1));
      allOdds.AddRange(playerBOdds.Where(x => x.DecimalOdd == playerBOdds.Max(m => m.DecimalOdd)).OrderBy(x => (50 - x.OddsSource.Length) + ((x.OddsSource.Length % 2) * 10)).Take(1));

      var ret = Mapper.Map<IEnumerable<OddsForEvent>, IEnumerable<OddViewModel>>(allOdds).ToList();
      ret.ForEach(x =>
      {
        x.MatchId = fixture.Id;
        x.Sport = this.sport;
      });

      return ret;
    }

    public IEnumerable<OddViewModel> GetAllTennisOdds(DateTime date, IEnumerable<TennisFixtureViewModel> fixtures)
    {
      var ret = new List<OddViewModel>();

      foreach (var fixture in fixtures)
      {
        ret.AddRange(GetSingleTennisOdds(date, fixture));
      }

      return ret;
    }

    public IEnumerable<OddViewModel> FetchTennisOddsForTournamentSource(DateTime date, TournamentViewModel tournament, OddsSourceViewModel oddsSource)
    {
      var urlCheck = this.bookmakerRepository.GetTournamentCouponUrl(tournament.TournamentName, oddsSource.Source);
      if (urlCheck == null)
      {
        //will have already been checked for the FetchAllTennisOddsVersion
        var missingURL = new MissingTournamentCouponURLObject { ExternalSource = oddsSource.Source, Tournament = tournament.TournamentName };
        throw new MissingTournamentCouponURLException(new MissingTournamentCouponURLObject[] { missingURL }, "Tournament coupons missing");
      }
      return FetchCoupons(date, tournament.TournamentName, oddsSource.Source, this.sport, true, false);
    }

    public IEnumerable<OddViewModel> FetchAllTennisOdds(DateTime date)
    {
      var odds = new List<OddViewModel>();
      var missingAlias = new List<MissingTeamPlayerAliasObject>();

      var tournaments = DaysTournaments(date, this.sport);
      var oddsSources = 
        this.bookmakerRepository
            .GetActiveOddsSources()
            .ToList();

      //check URL's exist first
      var urlCheck =
        tournaments.SelectMany(t => oddsSources.Where(s => this.bookmakerRepository.GetTournamentCouponUrl(t, s) == null)
                                               .Select(s => new MissingTournamentCouponURLObject() { ExternalSource = s.Source, Tournament = t.TournamentName }))
                   .ToList();

      if (urlCheck.Count() > 0)
        throw new MissingTournamentCouponURLException(urlCheck.ToList(), "Tournament coupons missing");

      foreach (var tournament in tournaments)
      {
        foreach (var source in oddsSources)
        {
          try
          {
            var tournamentViewModel = new TournamentViewModel { TournamentName = tournament.TournamentName };
            var oddsSourceViewModel = new OddsSourceViewModel { Source = source.Source };
            odds.AddRange(FetchTennisOddsForTournamentSource(date, tournamentViewModel, oddsSourceViewModel));
          }
          catch (MissingTeamPlayerAliasException mtpaEx)
          {
            missingAlias.AddRange(mtpaEx.MissingAlias);
          }
        }
      }
      if (missingAlias.Count > 0)
        throw new MissingTeamPlayerAliasException(missingAlias, "Missing team or player alias");
      return odds;
    }

    public IEnumerable<OddViewModel> FetchCoupons(DateTime date, string tournament, string oddsSource, string sport, bool getOdds, bool prescreen)
    {
      var coupons = FetchMatchCoupons(date, tournament, oddsSource, sport, getOdds, prescreen);
      var odds = Mapper.Map<IEnumerable<Model.GenericMatchCoupon>, IEnumerable<OddViewModel>>(coupons).ToList();
      odds.ForEach(x => x.Sport = this.sport);
      
      return odds;
    }

    protected override bool QualifiesPredicate(decimal probability, decimal odds, decimal edgeRequired, int gamesPlayed, int? minGamesRequired)
    {
      return base.QualifiesPredicate(probability, odds, edgeRequired, gamesPlayed, minGamesRequired) &&
        gamesPlayed >= (minGamesRequired ?? 0);
    }


  }
}
