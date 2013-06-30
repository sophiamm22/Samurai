using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Services.Contracts.Async;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Tennis;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Domain.Value.Async;
using Model = Samurai.Domain.Model;
using Samurai.Web.ViewModels.Value;
using Samurai.Domain.Exceptions;

namespace Samurai.Services.Async
{
  public class AsyncTennisOddsService : AsyncOddsService, IAsyncTennisOddsService
  {
    public AsyncTennisOddsService(IFixtureRepository fixtureRepository, IBookmakerRepository bookmakerRepository,
      IStoredProceduresRepository storedProcedureRepository, IPredictionRepository predictionRepository,
      IAsyncCouponStrategyProvider couponProvider, IAsyncOddsStrategyProvider oddsProvider)
      : base(fixtureRepository, bookmakerRepository, storedProcedureRepository, predictionRepository, 
      couponProvider, oddsProvider)
    {
      this.sport = "Tennis";
    }

    public async Task<IEnumerable<OddViewModel>> GetPeriodTennisOdds(DateTime startDate, DateTime endDate)
    {
      return await Task.Run(() => GetPeriodTennisOddsSync(startDate, endDate));
    }

    public async Task<IEnumerable<OddViewModel>> GetSingleTennisOdds(int matchID)
    {
      return await Task.Run(() => GetSingleTennisOddsSync(matchID));
    }

    public async Task<IEnumerable<OddViewModel>> GetAllTennisOdds(IEnumerable<int> matchIDs)
    {
      var ret = new List<OddViewModel>();

      foreach (var matchID in matchIDs)
      {
        ret.AddRange(await GetSingleTennisOdds(matchID));
      }

      return ret;
    }

    public async Task<IEnumerable<OddViewModel>> GetAllTennisTodaysOdds(DateTime fixtureDate)
    {
      var ids =
        this.fixtureRepository
            .GetDaysMatches(fixtureDate)
            .Where(x => x.TournamentEvent.Tournament.Competition.Sport.SportName == "Tennis")
            .Select(x => x.Id);

      return await GetAllTennisOdds(ids);
    }

    public async Task<IEnumerable<OddViewModel>> FetchAllTennisOdds(DateTime date)
    {
      var oddsViewModels = new List<OddViewModel>();
      var missingAlias = new List<MissingTeamPlayerAlias>();

      var tournaments = DaysTournaments(date, this.sport);
      var oddsSources =
        this.bookmakerRepository
            .GetActiveOddsSources()
            .ToList();

      //check URL's exist first
      var urlCheck =
        tournaments.SelectMany(t => oddsSources.Where(s => this.bookmakerRepository.GetTournamentCouponUrl(t, s) == null)
                                               .Select(s => new MissingTournamentCouponURL() { ExternalSource = s.Source, Tournament = t.TournamentName }))
                   .ToList();

      if (urlCheck.Count() > 0)
        throw new TournamentCouponURLMissingException(urlCheck.ToList(), "Tournament coupons missing");

      foreach (var tournament in tournaments)
      {
        foreach (var source in oddsSources)
        {
          try
          {
            var tournamentViewModel = new TournamentViewModel { TournamentName = tournament.TournamentName };
            var oddsSourceViewModel = new OddsSourceViewModel { Source = source.Source };
            oddsViewModels.AddRange(await FetchTennisOddsForTournamentSource(date, tournamentViewModel, oddsSourceViewModel));
          }
          catch (MissingTeamPlayerAliasException mtpaEx)
          {
            missingAlias.AddRange(mtpaEx.MissingAlias);
          }
        }
      }
      if (missingAlias.Count > 0)
        throw new MissingTeamPlayerAliasException(missingAlias, "Missing team or player alias");
      return oddsViewModels;
    }

    public async Task<IEnumerable<OddViewModel>> FetchTennisOddsForTournamentSource(DateTime date, TournamentViewModel tournament, OddsSourceViewModel oddsSource)
    {
      var urlCheck = 
        this.bookmakerRepository
            .GetTournamentCouponUrl(tournament.TournamentName, oddsSource.Source);
      if (urlCheck == null)
      {
        //will have already been checked for the FetchAllTennisOddsVersion
        var missingURL = new MissingTournamentCouponURL { ExternalSource = oddsSource.Source, Tournament = tournament.TournamentName };
        throw new TournamentCouponURLMissingException(new MissingTournamentCouponURL[] { missingURL }, "Tournament coupons missing");
      }
      return await FetchCoupons(date, tournament.TournamentName, oddsSource.Source);
    }

    public async Task<IEnumerable<OddViewModel>> FetchCoupons(DateTime date, string tournament, string oddsSource)
    {
      var coupons = await FetchMatchCoupons(date, tournament, oddsSource, this.sport);
      var odds = Mapper.Map<IEnumerable<Model.GenericMatchCoupon>, IEnumerable<OddViewModel>>(coupons).ToList();
      odds.ForEach(x => x.Sport = this.sport);
      return odds;
    }

    private IEnumerable<OddViewModel> GetPeriodTennisOddsSync(DateTime startDate, DateTime endDate)
    {
      var oddsSources =
        this.bookmakerRepository
            .GetActiveOddsSources()
            .Select(s => s.Source)
            .ToList();

      var allOdds = new List<OddsForEvent>();
      var oddsForPeriod =
        this.storedProcedureRepository
            .GetAllOddsForPeriod(startDate, endDate, oddsSources.First()) //come back to this
            .ToList();

      allOdds.AddRange(oddsForPeriod);

      var ret = Mapper.Map<IEnumerable<OddsForEvent>, IEnumerable<OddViewModel>>(allOdds).ToList();
      return ret;
    }

    private IEnumerable<OddViewModel> GetSingleTennisOddsSync(int matchID)
    {
      var oddsSources =
        this.bookmakerRepository
            .GetActiveOddsSources()
            .Select(s => s.Source)
            .ToList();

      var allOdds = new List<OddsForEvent>();
      var playerAOdds = new List<OddsForEvent>();
      var playerBOdds = new List<OddsForEvent>();

      foreach (var oddsSource in oddsSources)
      {
        var oddsForEvent =
          this.storedProcedureRepository
              .GetBestOddsFromMatchID(matchID, oddsSource)
              .ToList();

        playerAOdds.AddRange(oddsForEvent.Where(x => x.Outcome == "Home Win"));
        playerBOdds.AddRange(oddsForEvent.Where(x => x.Outcome == "Away Win"));
      }

      allOdds.AddRange(playerAOdds.Where(x => x.DecimalOdd == playerAOdds.Max(m => m.DecimalOdd)).OrderBy(x => (50 - x.OddsSource.Length) + ((x.OddsSource.Length % 2) * 10)).Take(1));
      allOdds.AddRange(playerBOdds.Where(x => x.DecimalOdd == playerBOdds.Max(m => m.DecimalOdd)).OrderBy(x => (50 - x.OddsSource.Length) + ((x.OddsSource.Length % 2) * 10)).Take(1));

      var ret = Mapper.Map<IEnumerable<OddsForEvent>, IEnumerable<OddViewModel>>(allOdds).ToList();
      ret.ForEach(x =>
        {
          x.MatchId = matchID;
          x.Sport = this.sport;
        });

      return ret;
    }
  }
}
