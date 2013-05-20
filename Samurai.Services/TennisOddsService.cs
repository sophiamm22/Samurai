﻿using System;
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

    public TennisCouponOutcomeViewModel GetSingleTennisOdds(DateTime date, TennisFixtureViewModel fixture)
    {
      var oddsSources =
        this.bookmakerRepository
            .GetActiveOddsSources()
            .ToList(); 

      var relatedOdds = new List<TennisCouponOutcomeViewModel>();

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

        var asCouponVM = Mapper.Map<IEnumerable<OddsForEvent>, TennisCouponOutcomeViewModel>(oddsForEvent);
        asCouponVM.MatchIdentifier = fixture.MatchIdentifier;
        asCouponVM.CouponURL = new Dictionary<string, string>();
        if (!(oddsForEvent.FirstOrDefault() == null || string.IsNullOrEmpty(oddsForEvent.First().MatchCouponURL)))
          asCouponVM.CouponURL.Add(oddsSource.Source, oddsForEvent.First().MatchCouponURL);

        relatedOdds.Add(asCouponVM);
      }

      var singleCouponVM = Mapper.Map<List<TennisCouponOutcomeViewModel>, TennisCouponOutcomeViewModel>(relatedOdds);
      singleCouponVM.MatchIdentifier = relatedOdds.First().MatchIdentifier;
      return singleCouponVM;
    }

    public IEnumerable<TennisCouponOutcomeViewModel> GetAllTennisOdds(DateTime date, IEnumerable<TennisFixtureViewModel> fixtures)
    {
      var ret = new List<TennisCouponOutcomeViewModel>();

      foreach (var fixture in fixtures)
      {
        ret.Add(GetSingleTennisOdds(date, fixture));
      }

      return ret;
    }

    public IEnumerable<TennisCouponOutcomeViewModel> FetchTennisOddsForTournamentSource(DateTime date, TournamentViewModel tournament, OddsSourceViewModel oddsSource)
    {
      var urlCheck = this.bookmakerRepository.GetTournamentCouponUrl(tournament.TournamentName, oddsSource.Source);
      if (urlCheck == null)
      {
        //will have already been checked for the FetchAllTennisOddsVersion
        var missingURL = new MissingTournamentCouponURL { ExternalSource = oddsSource.Source, Tournament = tournament.TournamentName };
        throw new TournamentCouponURLMissingException(new MissingTournamentCouponURL[] { missingURL }, "Tournament coupons missing");
      }
      return FetchCoupons(date, tournament.TournamentName, oddsSource.Source, this.sport, true, false);
    }

    public IEnumerable<TennisCouponOutcomeViewModel> FetchAllTennisOdds(DateTime date)
    {
      var matchCoupons = new List<TennisCouponOutcomeViewModel>();
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
            matchCoupons.AddRange(FetchTennisOddsForTournamentSource(date, tournamentViewModel, oddsSourceViewModel));
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

    public IEnumerable<TennisCouponOutcomeViewModel> FetchCoupons(DateTime date, string tournament, string oddsSource, string sport, bool getOdds, bool prescreen)
    {
      var coupons = FetchMatchCoupons(date, tournament, oddsSource, sport, getOdds, prescreen);
      return Mapper.Map<IEnumerable<Model.GenericMatchCoupon>, IEnumerable<TennisCouponOutcomeViewModel>>(coupons);
    }

    protected override bool QualifiesPredicate(decimal probability, decimal odds, decimal edgeRequired, int gamesPlayed, int? minGamesRequired)
    {
      return base.QualifiesPredicate(probability, odds, edgeRequired, gamesPlayed, minGamesRequired) &&
        gamesPlayed >= (minGamesRequired ?? 0);
    }


  }
}
