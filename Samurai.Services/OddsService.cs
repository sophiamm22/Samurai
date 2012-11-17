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
using Samurai.Domain.Value;
using Model = Samurai.Domain.Model;

namespace Samurai.Services
{
  public class OddsService : IOddsService
  {
    private readonly IFixtureRepository fixtureRepository;
    private readonly IBookmakerRepository bookmakerRepository;
    private readonly ICouponStrategyProvider couponProvider;
    private readonly IOddsStrategyProvider oddsProvider;

    public OddsService(IFixtureRepository fixtureRepository, IBookmakerRepository bookmakerRepository,
      ICouponStrategyProvider couponProvider, IOddsStrategyProvider oddsProvider)
    {
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (bookmakerRepository == null) throw new ArgumentNullException("bookmakerRepository");
      if (couponProvider == null) throw new ArgumentNullException("couponProvider");
      if (oddsProvider == null) throw new ArgumentNullException("oddsProvider");

      this.fixtureRepository = fixtureRepository;
      this.bookmakerRepository = bookmakerRepository;
      this.couponProvider = couponProvider;
      this.oddsProvider = oddsProvider;
    }

    public OddsSourceViewModel FindOddsSource(string slug)
    {
      var externalSource = this.bookmakerRepository.GetExternalSourceFromSlug(slug);
      if (externalSource == null) return null;
      return Mapper.Map<ExternalSource, OddsSourceViewModel>(externalSource);
    }

    public SportViewModel FindSport(string slug)
    {
      var sport = this.fixtureRepository.GetSport(slug);
      if (sport == null) return null;
      return Mapper.Map<Sport, SportViewModel>(sport);
    }

    public TournamentViewModel FindTournament(string slug)
    {
      var tournament = this.fixtureRepository.GetTournamentFromSlug(slug);
      if (tournament == null) return null;
      return Mapper.Map<Tournament, TournamentViewModel>(tournament);
    }

    public IEnumerable<FootballFixtureViewModel> FetchAllFootballOdds(DateTime date)
    {
      var matchCoupons = new List<FootballFixtureViewModel>();

      var tournaments = this.fixtureRepository.GetDaysFootballMatches(date)
                                              .Select(t => t.TournamentEvent.Tournament.TournamentName)
                                              .Distinct()
                                              .ToList();
      var sport = "Football";

      var oddsSources = this.bookmakerRepository.GetActiveOddsSources().Select(o => o.Source);

      foreach (var tournament in tournaments)
      {
        foreach (var source in oddsSources)
        {
          matchCoupons.AddRange(FetchCoupons(date, tournament, source, sport, true));
        }
      }
      return matchCoupons;
    }

    public IEnumerable<FootballFixtureViewModel> FetchCoupons(DateTime date, string tournament, string oddsSource, string sport, bool getOdds)
    {
      var valueOptions = GetValueOptions(date, tournament, oddsSource, sport);
      var couponStrategy = this.couponProvider.CreateCouponStrategy(valueOptions);

      var coupons = couponStrategy.GetMatches().ToList();
      //for odds checker mobile - no dates
      var todaysCoupons = coupons.Any(x => x.MatchDate == null) ? coupons.ToList() : coupons.Where(x => x.MatchDate.Date == date.Date).ToList();
      if (getOdds)
      {
        var oddsStrategy = this.oddsProvider.CreateOddsStrategy(valueOptions);
        var timeStamp = DateTime.Now;
        foreach (var coupon in todaysCoupons)
        {
          coupon.ActualOdds = oddsStrategy.GetOdds(coupon, timeStamp);
        }
      }
      var matches = PersistCoupons(todaysCoupons, date, tournament);
      return Mapper.Map<IEnumerable<Match>, IEnumerable<FootballFixtureViewModel>>(matches);
    }

    private IEnumerable<Match> PersistCoupons(IEnumerable<Model.IGenericMatchCoupon> coupons, DateTime couponDate, string tournament)
    {
      var matches = this.fixtureRepository.GetMatchesForOdds(couponDate, tournament);

      var sources = coupons.Select(c => c.Source).Distinct()
        .ToDictionary(s => s, s => this.bookmakerRepository.GetExternalSource(s));

      var bestOddsBookmaker = coupons.Select(c=>c.Source).Distinct()
        .ToDictionary(s => s + " Best Available", s => this.bookmakerRepository.FindByName(s + " Best Available"));

      foreach (var coupon in coupons)
      {
        var persistedMatch = matches.FirstOrDefault(m => m.TeamsPlayerA.TeamName == coupon.TeamOrPlayerA && 
                                                         m.TeamsPlayerB.TeamName == coupon.TeamOrPlayerB);
        if (persistedMatch == null) 
          continue; //need a better way to deal with this

        if (persistedMatch.MatchCouponURLs.Count(u => u.ExternalSource.Source == coupon.Source) == 0)
        {
          persistedMatch.MatchCouponURLs.Add(new MatchCouponURL()
          {
            ExternalSource = sources[coupon.Source],
            MatchCouponURLString = coupon.MatchURL.ToString()
          });
        }
        else
        {
          persistedMatch.MatchCouponURLs.First(u => u.ExternalSource.Source == coupon.Source).MatchCouponURLString = coupon.MatchURL.ToString();
        }

        if (persistedMatch.MatchOutcomeProbabilitiesInMatches.Count != Math.Max(coupon.HeadlineOdds.Count, coupon.ActualOdds.Count))
          continue; //need a better way to deal with this, some message passing back to the caller

        foreach (var outcome in coupon.HeadlineOdds.Keys)
        {
          var probForOutcome = persistedMatch.MatchOutcomeProbabilitiesInMatches.First(p => p.MatchOutcomeID == (int)outcome);
          var outcomeOdds = probForOutcome.MatchOutcomeOdds;
          var bestAvailableBookmaker = string.Format("{0} Best Available", coupon.Source);

          Func<MatchOutcomeOdd, bool> predicate = o => o.ExternalSource.Source == coupon.Source && 
                                                       o.Bookmaker.BookmakerName == bestAvailableBookmaker &&
                                                       o.TimeStamp == coupon.LastChecked;

          if (outcomeOdds.Count(predicate) == 0)
          {
            outcomeOdds.Add(new MatchOutcomeOdd()
            {
              Bookmaker = bestOddsBookmaker[bestAvailableBookmaker],
              ExternalSource = sources[coupon.Source],
              Odd = (decimal)coupon.HeadlineOdds[outcome],
              TimeStamp = coupon.LastChecked
            });
          }
          else //assume the same time and bookmaker is an update
          {
            var persistedOdd = outcomeOdds.First(predicate);
            persistedOdd.Odd = (decimal)coupon.HeadlineOdds[outcome];
          }
        }

        foreach (var outcome in coupon.ActualOdds.Keys)
        {
          var probForOutcome = persistedMatch.MatchOutcomeProbabilitiesInMatches.First(p => p.MatchOutcomeID == (int)outcome);
          var outcomeOdds = probForOutcome.MatchOutcomeOdds;

          foreach (var odd in coupon.ActualOdds[outcome])
          {
            Func<MatchOutcomeOdd, bool> predicate = o => o.ExternalSource.Source == coupon.Source &&
                                                         o.Bookmaker.BookmakerName == odd.BookmakerName &&
                                                         o.TimeStamp == coupon.LastChecked;

            if (outcomeOdds.Count(predicate) == 0)
            {
              outcomeOdds.Add(new MatchOutcomeOdd()
              {
                Bookmaker = this.bookmakerRepository.FindByName(odd.BookmakerName),
                ExternalSource = sources[coupon.Source],
                Odd = (decimal)odd.DecimalOdds,
                TimeStamp = odd.TimeStamp,
                ClickThroughURL = odd.ClickThroughURL.ToString()
              });
            }
            else
            {
              var persistedOdd = outcomeOdds.First(predicate);
              persistedOdd.Odd = (decimal)odd.DecimalOdds;
            }
          }
        }
      }
      this.fixtureRepository.SaveChanges();
      return matches;
    }

    

    private Model.IValueOptions GetValueOptions(DateTime date, string tournamentString, string oddsSourceString, string sportString)
    {      
      var tournament = fixtureRepository.GetTournament(tournamentString);
      var sport = this.fixtureRepository.GetSport(sportString);
      var source = this.bookmakerRepository.GetExternalSource(oddsSourceString);

      var valueOptions = new Model.ValueOptions()
      {
        CouponDate = date,
        Sport = sport,
        OddsSource = source,
        Tournament = tournament
      };

      return valueOptions;
    }
  }
}
