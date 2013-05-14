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

    public async Task<TennisCouponViewModel> GetSingleTennisOdds(int matchID)
    {
      return await Task.Run(() => GetSingleTennisOddsSync(matchID));
    }

    public async Task<IEnumerable<TennisCouponViewModel>> GetAllTennisOdds(IEnumerable<int> matchIDs)
    {
      var ret = new List<TennisCouponViewModel>();

      foreach (var matchID in matchIDs)
      {
        ret.Add(await GetSingleTennisOdds(matchID));
      }

      return ret;
    }

    public async Task<IEnumerable<TennisCouponViewModel>> GetAllTennisTodaysOdds(DateTime fixtureDate)
    {
      var ids =
        this.fixtureRepository
            .GetDaysMatches(fixtureDate)
            .Where(x => x.TournamentEvent.Tournament.Competition.Sport.SportName == "Tennis")
            .Select(x => x.Id);

      return await GetAllTennisOdds(ids);
    }

    public async Task<IEnumerable<TennisCouponViewModel>> FetchAllTennisOdds(DateTime date)
    {
      var matchCoupons = new List<TennisCouponViewModel>();
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
            matchCoupons.AddRange(await FetchTennisOddsForTournamentSource(date, tournamentViewModel, oddsSourceViewModel));
          }
          catch (MissingTeamPlayerAliasException mtpaEx)
          {
            missingAlias.AddRange(mtpaEx.MissingAlias);
          }
        }
      }
      if (missingAlias.Count > 0)
        throw new MissingTeamPlayerAliasException(missingAlias, "Missing team or player alias");
      return matchCoupons;
    }

    public async Task<IEnumerable<TennisCouponViewModel>> FetchTennisOddsForTournamentSource(DateTime date, TournamentViewModel tournament, OddsSourceViewModel oddsSource)
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

    public async Task<IEnumerable<TennisCouponViewModel>> FetchCoupons(DateTime date, string tournament, string oddsSource)
    {
      var coupons = await FetchMatchCoupons(date, tournament, oddsSource, this.sport);
      return Mapper.Map<IEnumerable<Model.GenericMatchCoupon>, IEnumerable<TennisCouponViewModel>>(coupons);
    }

    private TennisCouponViewModel GetSingleTennisOddsSync(int matchID)
    {
      var oddsSources =
        this.bookmakerRepository
            .GetActiveOddsSources()
            .Select(s => s.Source)
            .ToList();

      var relatedOdds = new List<TennisCouponViewModel>();

      foreach (var oddsSource in oddsSources)
      {
        var oddsForEvent =
          this.storedProcedureRepository
              .GetBestOddsFromMatchID(matchID, oddsSource)
              .ToList();

        var asCouponVM = Mapper.Map<IEnumerable<OddsForEvent>, TennisCouponViewModel>(oddsForEvent);

        asCouponVM.CouponURL = new Dictionary<string, string>();
        if (!(oddsForEvent.FirstOrDefault() == null || string.IsNullOrEmpty(oddsForEvent.First().MatchCouponURL)))
          asCouponVM.CouponURL.Add(oddsSource, oddsForEvent.First().MatchCouponURL);

        relatedOdds.Add(asCouponVM);
      }

      var singleCouponVM = Mapper.Map<List<TennisCouponViewModel>, TennisCouponViewModel>(relatedOdds);
      singleCouponVM.MatchIdentifier = relatedOdds.First().MatchIdentifier;
      singleCouponVM.MatchId = matchID;

      var bestOdds = new TennisCouponViewModel()
      {
        HomeWin = singleCouponVM.HomeWin.Where(x => x.DecimalOdd == singleCouponVM.HomeWin.Max(m => m.DecimalOdd)).OrderBy(x => (50 - x.OddsSource.Length) + ((x.OddsSource.Length % 2) * 10)).Take(1), //ugly hack to order by the preference Oddschecker.com -> Bestbetting -> Oddschecker.mobi
        AwayWin = singleCouponVM.AwayWin.Where(x => x.DecimalOdd == singleCouponVM.AwayWin.Max(m => m.DecimalOdd)).OrderBy(x => (50 - x.OddsSource.Length) + ((x.OddsSource.Length % 2) * 10)).Take(1),
        MatchId = matchID,
        MatchIdentifier = relatedOdds.First().MatchIdentifier
      };

      return bestOdds;
    }
  }
}
