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

    public async Task<TennisCouponViewModel> GetSingleTennisOdds(DateTime date, TennisFixtureViewModel fixture)
    {
      return await Task.Run(() => GetSingleTennisOddsSync(date, fixture));
    }

    public async Task<IEnumerable<TennisCouponViewModel>> GetAllTennisOdds(DateTime date, IEnumerable<TennisFixtureViewModel> fixtures)
    {
      var ret = new List<TennisCouponViewModel>();

      foreach (var fixture in fixtures)
      {
        ret.Add(await GetSingleTennisOdds(date, fixture));
      }

      return ret;
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

    private TennisCouponViewModel GetSingleTennisOddsSync(DateTime date, TennisFixtureViewModel fixture)
    {
      var oddsSources =
        this.bookmakerRepository
            .GetActiveOddsSources()
            .ToList();

      var relatedOdds = new List<TennisCouponViewModel>();

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

        var asCouponVM = Mapper.Map<IEnumerable<OddsForEvent>, TennisCouponViewModel>(oddsForEvent);
        asCouponVM.MatchIdentifier = fixture.MatchIdentifier;
        asCouponVM.CouponURL = new Dictionary<string, string>();
        if (!(oddsForEvent.FirstOrDefault() == null || string.IsNullOrEmpty(oddsForEvent.First().MatchCouponURL)))
          asCouponVM.CouponURL.Add(oddsSource.Source, oddsForEvent.First().MatchCouponURL);

        relatedOdds.Add(asCouponVM);
      }

      var singleCouponVM = Mapper.Map<List<TennisCouponViewModel>, TennisCouponViewModel>(relatedOdds);
      singleCouponVM.MatchIdentifier = relatedOdds.First().MatchIdentifier;
      return singleCouponVM;
    }
  }
}
