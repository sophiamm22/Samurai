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
  public class OddsService : IOddsService
  {
    private readonly IFixtureRepository fixtureRepository;
    private readonly IBookmakerRepository bookmakerRepository;
    private readonly IStoredProceduresRepository storedProcedureRepository;
    private readonly ICouponStrategyProvider couponProvider;
    private readonly IOddsStrategyProvider oddsProvider;

    private List<Model.IGenericMatchCoupon> prescreenedCouponTarget;

    public OddsService(IFixtureRepository fixtureRepository, IBookmakerRepository bookmakerRepository,
      IStoredProceduresRepository storedProcedureRepository,
      ICouponStrategyProvider couponProvider, IOddsStrategyProvider oddsProvider)
    {
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (bookmakerRepository == null) throw new ArgumentNullException("bookmakerRepository");
      if (storedProcedureRepository == null) throw new ArgumentNullException("storedProcedureRepository");
      if (couponProvider == null) throw new ArgumentNullException("couponProvider");
      if (oddsProvider == null) throw new ArgumentNullException("oddsProvider");

      this.fixtureRepository = fixtureRepository;
      this.bookmakerRepository = bookmakerRepository;
      this.storedProcedureRepository = storedProcedureRepository;
      this.couponProvider = couponProvider;
      this.oddsProvider = oddsProvider;

      this.prescreenedCouponTarget = new List<Model.IGenericMatchCoupon>();
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

    public IEnumerable<FootballFixtureViewModel> FetchAllPreScreenedFootballOdds(DateTime date)
    {
      var matchCoupons = new List<Match>();
      var matchCouponsQualifying = new List<Match>();
      var matchCouponsNotQualifying = new List<Match>();

      var tournaments = DaysTournaments(date);
      var sport = "Football";
      var oddsSources = this.bookmakerRepository.GetActiveOddsSources()
                                                .OrderByDescending(s => s.PrescreenDecider)
                                                .Select(o => o.Source)
                                                .ToList(); //order by the prescreen decider.  Most likely BestBetting.  
                                                //This is a bit messy but based on what is cached in the text versions of th HTML files

      foreach (var tournament in tournaments)
      {
        foreach (var source in oddsSources)
        {
          matchCoupons.AddRange(FetchMatchCoupons(date, tournament, source, sport, false, true));
        }
      }

      return Mapper.Map<IEnumerable<Match>, IEnumerable<FootballFixtureViewModel>>(matchCoupons);
    }

    public IEnumerable<FootballFixtureViewModel> FetchAllFootballOdds(DateTime date)
    {
      var matchCoupons = new List<FootballFixtureViewModel>();

      var tournaments = DaysTournaments(date);
      var sport = "Football";
      var oddsSources = this.bookmakerRepository.GetActiveOddsSources().Select(o => o.Source);

      foreach (var tournament in tournaments)
      {
        foreach (var source in oddsSources)
        {
          matchCoupons.AddRange(FetchCoupons(date, tournament, source, sport, true, false));
        }
      }
      return matchCoupons;
    }

    public IEnumerable<FootballFixtureViewModel> FetchCoupons(DateTime date, string tournament, string oddsSource, string sport, bool getOdds, bool prescreen)
    {
      var matches = FetchMatchCoupons(date, tournament, oddsSource, sport, getOdds, prescreen);
      return Mapper.Map<IEnumerable<Match>, IEnumerable<FootballFixtureViewModel>>(matches);
    }

    private IEnumerable<string> DaysTournaments(DateTime date)
    {
      var tournaments = this.fixtureRepository.GetDaysFootballMatches(date)
                                              .Select(t => t.TournamentEvent.Tournament.TournamentName)
                                              .Distinct()
                                              .ToList();
      return tournaments;
    }

    private IEnumerable<Match> FetchMatchCoupons(DateTime date, string tournament, string oddsSource, string sport, bool getOdds, bool preScreenOdds)
    {
      var valueOptions = GetValueOptions(date, tournament, oddsSource, sport);
      var couponStrategy = this.couponProvider.CreateCouponStrategy(valueOptions);

      var coupons = couponStrategy.GetMatches().ToList();
      //for odds checker mobile - no dates
      var todaysCoupons = coupons.Any(x => x.MatchDate == null) ? coupons.ToList() : coupons.Where(x => x.MatchDate.Date == date.Date).ToList();

      IEnumerable<Model.IGenericMatchCoupon> getOddsFor = null;
      IEnumerable<Model.IGenericMatchCoupon> dontGetOddsFor = null;

      if (getOdds)
      {
        if (preScreenOdds)
        {
          PrescreenFootballCoupons(date, todaysCoupons, out getOddsFor, out dontGetOddsFor);
        }
        else
          getOddsFor = todaysCoupons;

        var oddsStrategy = this.oddsProvider.CreateOddsStrategy(valueOptions);
        var timeStamp = DateTime.Now;
        foreach (var coupon in todaysCoupons)
        {
          coupon.ActualOdds = oddsStrategy.GetOdds(coupon, timeStamp);
        }
      }
      else
      {
        dontGetOddsFor = todaysCoupons;
      }

      if (valueOptions.OddsSource.PrescreenDecider)
        this.prescreenedCouponTarget.AddRange(getOddsFor);

      var matchesOdds = getOddsFor == null || getOddsFor.Count() == 0 ? Enumerable.Empty<Match>() : PersistCoupons(getOddsFor, date, tournament);
      var matchesNoOdds = dontGetOddsFor == null || dontGetOddsFor.Count() == 0 ? Enumerable.Empty<Match>() : PersistCoupons(dontGetOddsFor, date, tournament);

      var matches = new List<Match>();
      matches.AddRange(matchesOdds);
      matches.AddRange(matchesNoOdds);
      return matches;
    }

    private void PrescreenFootballCoupons(DateTime date, IEnumerable<Model.IGenericMatchCoupon> allCoupons,
      out IEnumerable<Model.IGenericMatchCoupon> getOddsFor, out IEnumerable<Model.IGenericMatchCoupon> dontGetOddsFor)
    {
      var getOddsForReturn = new List<Model.IGenericMatchCoupon>();
      var dontGetOddsForReturn = new List<Model.IGenericMatchCoupon>();

      var probabilities = this.storedProcedureRepository
                              .GetOutcomeProbabilitiesForSport(date, "Football")
                              .ToDictionary(p => string.Format("{0}|{1}", p.HomeTeam, p.AwayTeam));

      var coupons = allCoupons.ToDictionary(c => string.Format("{0}|{1}", c.TeamOrPlayerA, c.TeamOrPlayerB));
      var prescreenedCoupons = this.prescreenedCouponTarget.ToDictionary(c => string.Format("{0}|{1}", c.TeamOrPlayerA, c.TeamOrPlayerB));

      foreach (var couponKey in coupons.Keys)
      {
        if (probabilities.ContainsKey(couponKey))
        {
          var probabilitySet = probabilities[couponKey];
          var qualifies = false;
          if (prescreenedCoupons.Count != 0 && prescreenedCoupons.ContainsKey(couponKey))
          {
            qualifies = true;
          }
          else
          {
            foreach (var oddKVP in coupons[couponKey].HeadlineOdds)
            {
              var probability = probabilitySet.OutcomeProbabilties[(int)oddKVP.Key];
              qualifies = qualifies || (probability * (decimal)oddKVP.Value - 1 >= probabilitySet.EdgeRequired);
            }
          }
          if (qualifies)
            getOddsForReturn.Add(coupons[couponKey]);
          else
            dontGetOddsForReturn.Add(coupons[couponKey]);
        }
      }

      getOddsFor = getOddsForReturn;
      dontGetOddsFor = dontGetOddsForReturn;
    }

    private IEnumerable<Match> FetchOddsForMatches(Model.IValueOptions valueOptions, IEnumerable<Match> matches)
    {
      var coupons = new List<Model.IGenericMatchCoupon>();
      var oddsStrategy = this.oddsProvider.CreateOddsStrategy(valueOptions);
      var timeStamp = DateTime.Now;

      foreach (var match in matches)
      {
        var coupon = new Model.GenericMatchCoupon
        {
          TeamOrPlayerA = match.TeamsPlayerA.TeamName,
          TeamOrPlayerB = match.TeamsPlayerB.TeamName,
          MatchURL = new Uri(match.MatchCouponURLs.First(m => m.ExternalSource.Source == valueOptions.OddsSource.Source).MatchCouponURLString),
        };
        coupon.ActualOdds = oddsStrategy.GetOdds(coupon, timeStamp);
        coupons.Add(coupon);
      }
      var matchesReturn = PersistCoupons(coupons, valueOptions.CouponDate, valueOptions.Tournament.TournamentName);
      return matchesReturn;
    }

    private IEnumerable<Match> PersistCoupons(IEnumerable<Model.IGenericMatchCoupon> coupons, DateTime couponDate, string tournament)
    {
      var matches = this.fixtureRepository.GetMatchesForOdds(couponDate, tournament);

      var sources = coupons.Select(c => c.Source).Distinct()
        .ToDictionary(s => s, s => this.bookmakerRepository.GetExternalSource(s));

      var bestOddsBookmaker = coupons.Select(c => c.Source).Distinct()
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
            MatchCouponURLString = coupon.MatchURL == null ? string.Empty : coupon.MatchURL.ToString()
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
            if (bestOddsBookmaker[bestAvailableBookmaker] == null)
              Console.WriteLine("We're screwed");
            var matchOutcomeOdd = new MatchOutcomeOdd()
            {
              Bookmaker = bestOddsBookmaker[bestAvailableBookmaker],
              ExternalSource = sources[coupon.Source],
              Odd = (decimal)coupon.HeadlineOdds[outcome],
              TimeStamp = coupon.LastChecked
            };
            outcomeOdds.Add(matchOutcomeOdd);
            this.bookmakerRepository.AddMatchOutcomeOdd(matchOutcomeOdd);
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
                                                        (o.Bookmaker == null ||
                                                         o.Bookmaker.BookmakerName == odd.BookmakerName) &&
                                                         o.TimeStamp == coupon.LastChecked;

            if (outcomeOdds.Count(predicate) == 0)
            {
              if (this.bookmakerRepository.FindByName(odd.BookmakerName) == null)
                Console.WriteLine("We're screwed");
              var matchOutcomeOdd = new MatchOutcomeOdd()
              {
                Bookmaker = this.bookmakerRepository.FindByName(odd.BookmakerName),
                ExternalSource = sources[coupon.Source],
                Odd = (decimal)odd.DecimalOdds,
                TimeStamp = odd.TimeStamp,
                //ClickThroughURL = odd.ClickThroughURL.ToString()
              };
              outcomeOdds.Add(matchOutcomeOdd);
              this.bookmakerRepository.AddMatchOutcomeOdd(matchOutcomeOdd);
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
