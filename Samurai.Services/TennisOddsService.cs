﻿using System;
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

    public IEnumerable<TennisCouponViewModel> GetAllTennisOdds(DateTime date, IEnumerable<TennisFixtureViewModel> fixtures)
    {
      var ret = new List<TennisCouponViewModel>();

      var oddsSources = 
        this.bookmakerRepository
            .GetActiveOddsSources()
            .ToList();

      foreach (var fixture in fixtures)
      {
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
        ret.Add(Mapper.Map<List<TennisCouponViewModel>, TennisCouponViewModel>(relatedOdds));
      }

      return ret;
    }

    public IEnumerable<TennisCouponViewModel> FetchAllTennisOddsNew(DateTime date)
    {
      var matchCoupons = new List<TennisCouponViewModel>();
      var missingAlias = new List<MissingTeamPlayerAlias>();

      var tournaments = DaysTournaments(date, this.sport);
      var oddsSources = this.bookmakerRepository.GetActiveOddsSources().ToList();

      //check URL's exist first
      var urlCheck =
        tournaments.SelectMany(t => oddsSources.Where(s => this.bookmakerRepository.GetTournamentCouponUrl(t, s) == null)
                                               .Select(s => new MissingTournamentCouponURL() { ExternalSource = s.Source, Tournament = t.TournamentName }))
                   .ToList();

      if (urlCheck.Count() > 0)
        throw new TournamentCouponURLMissingException(urlCheck.ToList(), "Tournament coupons missing");


      foreach (var tournament in tournaments.Select(t => t.TournamentName))
      {
        foreach (var source in oddsSources.Select(o => o.Source))
        {
          try
          {
            matchCoupons.AddRange(FetchCouponsNew(date, tournament, source, this.sport, true, false));
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

    public IEnumerable<TennisMatchViewModel> FetchAllTennisOdds(DateTime date)
    {
      var matchCoupons = new List<TennisMatchViewModel>();
      var missingAlias = new List<MissingTeamPlayerAlias>();

      var tournaments = DaysTournaments(date, this.sport).ToList();
      var oddsSources = this.bookmakerRepository.GetActiveOddsSources().ToList();

      //check URL's exist first
      var urlCheck =
        tournaments.SelectMany(t => oddsSources.Where(s => this.bookmakerRepository.GetTournamentCouponUrl(t, s) == null)
                                               .Select(s => new MissingTournamentCouponURL() { ExternalSource = s.Source, Tournament = t.TournamentName }));
      if (urlCheck.Count() > 0)
        throw new TournamentCouponURLMissingException(urlCheck, "Tournament coupons missing");
            
      foreach (var tournament in tournaments.Select(t => t.TournamentName))
      {
        foreach (var source in oddsSources.Select(o => o.Source))
        {
          try
          {
            matchCoupons.AddRange(FetchCoupons(date, tournament, source, this.sport, true, false));
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
