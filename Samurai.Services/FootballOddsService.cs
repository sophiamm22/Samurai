using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Domain.Value;
using Model = Samurai.Domain.Model;
using Samurai.Web.ViewModels.Value;

namespace Samurai.Services
{
  public abstract class OddsService : IOddsService
  {
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly IBookmakerRepository bookmakerRepository;
    protected readonly IStoredProceduresRepository storedProcedureRepository;
    protected readonly IPredictionRepository predicitonRepository;
    protected readonly ICouponStrategyProvider couponProvider;
    protected readonly IOddsStrategyProvider oddsProvider;

    protected string sport;

    protected List<Model.GenericMatchCoupon> prescreenedCouponTarget;

    public OddsService(IFixtureRepository fixtureRepository, IBookmakerRepository bookmakerRepository,
      IStoredProceduresRepository storedProcedureRepository, IPredictionRepository predicitonRepository,
      ICouponStrategyProvider couponProvider, IOddsStrategyProvider oddsProvider)
    {
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (bookmakerRepository == null) throw new ArgumentNullException("bookmakerRepository");
      if (storedProcedureRepository == null) throw new ArgumentNullException("storedProcedureRepository");
      if (predicitonRepository == null) throw new ArgumentNullException("predictionRepository");
      if (couponProvider == null) throw new ArgumentNullException("couponProvider");
      if (oddsProvider == null) throw new ArgumentNullException("oddsProvider");

      this.fixtureRepository = fixtureRepository;
      this.bookmakerRepository = bookmakerRepository;
      this.storedProcedureRepository = storedProcedureRepository;
      this.predicitonRepository = predicitonRepository;
      this.couponProvider = couponProvider;
      this.oddsProvider = oddsProvider;

      this.prescreenedCouponTarget = new List<Model.GenericMatchCoupon>();
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

    protected IEnumerable<Tournament> DaysTournaments(DateTime date, string sport)
    {
      var tournaments = this.fixtureRepository.GetDaysTournaments(date, sport);

      return tournaments;
    }

    //a total mess but going forward I will be getting all odds anyway so this is just for backwards compatability with my persisted data...
    protected IEnumerable<Model.GenericMatchCoupon> FetchMatchCouponsNew(DateTime date, string tournament, string oddsSource, string sport, bool getOdds, bool preScreenOdds)
    {
      var valueOptions = GetValueOptions(date, tournament, oddsSource, sport);
      var couponStrategy = this.couponProvider.CreateCouponStrategy(valueOptions);
      var tournamentEvent = this.fixtureRepository.GetTournamentEventFromTournamentAndDate(date, tournament);

      var coupons = couponStrategy.GetMatches().ToList();
      coupons.ForEach(x => x.TournamentEventName = tournamentEvent.EventName);
      //for odds checker mobile - no dates
      var todaysCoupons = coupons.Any(x => x.MatchDate == null || x.MatchDate == new DateTime()) ? coupons.ToList() : coupons.Where(x => x.MatchDate.Date == date.Date).ToList();

      IEnumerable<Model.GenericMatchCoupon> getOddsFor = null;
      IEnumerable<Model.GenericMatchCoupon> dontGetOddsFor = null;

      if (getOdds)
      {
        if (preScreenOdds)
        {
          PrescreenCoupons(date, todaysCoupons, out getOddsFor, out dontGetOddsFor);
        }
        else
          getOddsFor = todaysCoupons;

        var oddsStrategy = this.oddsProvider.CreateOddsStrategy(valueOptions);
        var timeStamp = DateTime.Now;
        foreach (var coupon in getOddsFor)
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

      var matchesOdds = getOddsFor == null || getOddsFor.Count() == 0 ? Enumerable.Empty<Model.GenericMatchCoupon>() : PersistCouponsNew(getOddsFor, date, tournament);
      var matchesNoOdds = dontGetOddsFor == null || dontGetOddsFor.Count() == 0 ? Enumerable.Empty<Model.GenericMatchCoupon>() : PersistCouponsNew(dontGetOddsFor, date, tournament);

      var matches = new List<Model.GenericMatchCoupon>();
      matches.AddRange(matchesOdds);
      matches.AddRange(matchesNoOdds);
      return matches;
    }

    //a total mess but going forward I will be getting all odds anyway so this is just for backwards compatability with my persisted data...
    protected IEnumerable<Match> FetchMatchCoupons(DateTime date, string tournament, string oddsSource, string sport, bool getOdds, bool preScreenOdds)
    {
      var valueOptions = GetValueOptions(date, tournament, oddsSource, sport);
      var couponStrategy = this.couponProvider.CreateCouponStrategy(valueOptions);

      var coupons = couponStrategy.GetMatches().ToList();
      //for odds checker mobile - no dates
      var todaysCoupons = coupons.Any(x => x.MatchDate == null || x.MatchDate == new DateTime()) ? coupons.ToList() : coupons.Where(x => x.MatchDate.Date == date.Date).ToList();

      IEnumerable<Model.GenericMatchCoupon> getOddsFor = null;
      IEnumerable<Model.GenericMatchCoupon> dontGetOddsFor = null;

      if (getOdds)
      {
        if (preScreenOdds)
        {
          PrescreenCoupons(date, todaysCoupons, out getOddsFor, out dontGetOddsFor);
        }
        else
          getOddsFor = todaysCoupons;

        var oddsStrategy = this.oddsProvider.CreateOddsStrategy(valueOptions);
        var timeStamp = DateTime.Now;
        foreach (var coupon in getOddsFor)
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

    protected void PrescreenCoupons(DateTime date, IEnumerable<Model.GenericMatchCoupon> allCoupons,
      out IEnumerable<Model.GenericMatchCoupon> getOddsFor, out IEnumerable<Model.GenericMatchCoupon> dontGetOddsFor)
    {
      var getOddsForReturn = new List<Model.GenericMatchCoupon>();
      var dontGetOddsForReturn = new List<Model.GenericMatchCoupon>();

      var probabilities = this.storedProcedureRepository
                              .GetOutcomeProbabilitiesForSport(date, this.sport)
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
              qualifies = qualifies || QualifiesPredicate(probability, 
                                                          (decimal)oddKVP.Value, 
                                                          probabilitySet.EdgeRequired, 
                                                          Math.Min(probabilitySet.GamesPlayedA ?? 0, probabilitySet.GamesPlayedB ?? 0),
                                                          probabilitySet.GamesRequiredForBet);
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

    protected virtual bool QualifiesPredicate(decimal probability, decimal odds, decimal edgeRequired, int gamesPlayed, int? minGamesRequired)
    {
      return probability * (decimal)odds - 1 >= edgeRequired;
    }

    protected IEnumerable<Model.GenericMatchCoupon> FetchOddsForMatchesNew(Model.IValueOptions valueOptions, IEnumerable<Match> matches)
    {
      var coupons = new List<Model.GenericMatchCoupon>();
      var oddsStrategy = this.oddsProvider.CreateOddsStrategy(valueOptions);
      var timeStamp = DateTime.Now;

      var tournamentEventIDs = matches.Select(m => m.TournamentEventID)
                                      .Distinct()
                                      .ToDictionary(t => t, t => this.fixtureRepository.GetTournamentEventById(t).EventName);


      foreach (var match in matches)
      {
        var teamOrPlayerA = this.fixtureRepository.GetTeamOrPlayerById(match.TeamAID);
        var teamOrPlayerB = this.fixtureRepository.GetTeamOrPlayerById(match.TeamBID);
        var matchCouponURLs = this.bookmakerRepository
                                  .GetMatchCouponURLs(match.Id)
                                  .First(m => m.ExternalSource.Source == valueOptions.OddsSource.Source)
                                  .MatchCouponURLString;

        var coupon = new Model.GenericMatchCoupon
        {
          TeamOrPlayerA = teamOrPlayerA.Name,
          FirstNameA = teamOrPlayerA.FirstName,
          TeamOrPlayerB = teamOrPlayerB.Name,
          FirstNameB = teamOrPlayerB.FirstName,
          TournamentEventName = tournamentEventIDs[match.TournamentEventID],
          MatchURL = new Uri(match.MatchCouponURLs.First(m => m.ExternalSource.Source == valueOptions.OddsSource.Source).MatchCouponURLString)
        };
        coupon.ActualOdds = oddsStrategy.GetOdds(coupon, timeStamp);
        coupons.Add(coupon);
      }
      var matchesReturn = PersistCoupons(coupons, valueOptions.CouponDate, valueOptions.Tournament.TournamentName);
      return coupons;
    }

    protected IEnumerable<Match> FetchOddsForMatches(Model.IValueOptions valueOptions, IEnumerable<Match> matches)
    {
      var coupons = new List<Model.GenericMatchCoupon>();
      var oddsStrategy = this.oddsProvider.CreateOddsStrategy(valueOptions);
      var timeStamp = DateTime.Now;

      foreach (var match in matches)
      {
        var coupon = new Model.GenericMatchCoupon
        {
          TeamOrPlayerA = match.TeamsPlayerA.Name,
          FirstNameA = (!string.IsNullOrEmpty(match.TeamsPlayerA.FirstName)) ? match.TeamsPlayerA.FirstName : null,
          TeamOrPlayerB = match.TeamsPlayerB.Name,
          FirstNameB = (!string.IsNullOrEmpty(match.TeamsPlayerB.FirstName)) ? match.TeamsPlayerB.FirstName : null,
          TournamentEventName = match.TournamentEvent.EventName,
          MatchURL = new Uri(match.MatchCouponURLs.First(m => m.ExternalSource.Source == valueOptions.OddsSource.Source).MatchCouponURLString),
        };
        coupon.ActualOdds = oddsStrategy.GetOdds(coupon, timeStamp);
        coupons.Add(coupon);
      }
      var matchesReturn = PersistCoupons(coupons, valueOptions.CouponDate, valueOptions.Tournament.TournamentName);
      return matchesReturn;
    }

    protected IEnumerable<Model.GenericMatchCoupon> PersistCouponsNew(IEnumerable<Model.GenericMatchCoupon> coupons, DateTime couponDate, string tournament)
    {
      var ret = new List<Model.GenericMatchCoupon>();
      var matches = this.fixtureRepository.GetMatchesForTournament(couponDate, tournament);

      var sources = coupons.Select(c => c.Source).Distinct()
        .ToDictionary(s => s, s => this.bookmakerRepository.GetExternalSource(s));

      var bestOddsBookmaker = coupons.Select(c => c.Source).Distinct()
        .ToDictionary(s => s + " Best Available", s => this.bookmakerRepository.FindByName(s + " Best Available"));

      foreach (var coupon in coupons)
      {
        var teamPlayerA = this.fixtureRepository.GetTeamOrPlayerFromName(coupon.TeamOrPlayerA);
        var teamPlayerB = this.fixtureRepository.GetTeamOrPlayerFromName(coupon.TeamOrPlayerB);
        var persistedMatch = this.fixtureRepository.GetMatchFromTeamSelections(teamPlayerA, teamPlayerB, couponDate);
        
        if (persistedMatch == null)
          continue; //won't get added to the return list but needs some reporting to the client

        var matchCouponURLs = this.bookmakerRepository
                                  .GetMatchCouponURLs(persistedMatch.Id)
                                  .Where(m => m.ExternalSource.Source == coupon.Source)
                                  .ToList();

        if (matchCouponURLs.Count == 0)
        {
          var newMatchCouponURL = new MatchCouponURL()
          {
            MatchID = persistedMatch.Id,
            ExternalSource = sources[coupon.Source],
            MatchCouponURLString = coupon.MatchURL == null ? string.Empty : coupon.MatchURL.ToString()
          };
          this.bookmakerRepository.AddMatchCouponURL(newMatchCouponURL);
        }
        else
        {
          matchCouponURLs.First(u => u.ExternalSource.Source == coupon.Source).MatchCouponURLString = coupon.MatchURL == null ? string.Empty : coupon.MatchURL.ToString();
        }

        var outcomeProbs = this.predicitonRepository
                               .GetMatchOutcomeProbabilities(persistedMatch.Id)
                               .ToList();
        
        if (outcomeProbs.Count == 0)
          continue; //need a better way to deal with this, some message passing back to the caller

        foreach (var outcome in coupon.HeadlineOdds.Keys)
        {
          var probForOutcome = outcomeProbs.First(p => p.MatchOutcomeID == (int)outcome);

          var outcomeOdds = this.bookmakerRepository
                                 .GetMatchOutcomeOdds(probForOutcome.Id)
                                 .ToList();

          var bestAvailableBookmaker = string.Format("{0} Best Available", coupon.Source);

          Func<MatchOutcomeOdd, bool> predicate = o => o.ExternalSource.Source == coupon.Source &&
                                                       o.Bookmaker.BookmakerName == bestAvailableBookmaker &&
                                                       o.TimeStamp == coupon.LastChecked;

          if (outcomeOdds.Count(predicate) == 0)
          {
            var matchOutcomeOdd = new MatchOutcomeOdd()
            {
              MatchOutcomeProbabilitiesInMatchID = probForOutcome.Id,
              Bookmaker = bestOddsBookmaker[bestAvailableBookmaker],
              ExternalSource = sources[coupon.Source],
              Odd = (decimal)coupon.HeadlineOdds[outcome],
              TimeStamp = coupon.LastChecked
            };

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
          var probForOutcome = outcomeProbs.First(p => p.MatchOutcomeID == (int)outcome);

          var outcomeOdds = this.bookmakerRepository
                                 .GetMatchOutcomeOdds(probForOutcome.Id)
                                 .ToList();

          foreach (var odd in coupon.ActualOdds[outcome])
          {
            Func<MatchOutcomeOdd, bool> predicate = o => o.ExternalSource.Source == coupon.Source &&
                                                        (o.Bookmaker == null ||
                                                         o.Bookmaker.BookmakerName == odd.BookmakerName) &&
                                                         o.TimeStamp == coupon.LastChecked;

            if (outcomeOdds.Count(predicate) == 0)
            {
              var matchOutcomeOdd = new MatchOutcomeOdd()
              {
                MatchOutcomeProbabilitiesInMatchID = probForOutcome.Id,
                Bookmaker = this.bookmakerRepository.FindByName(odd.BookmakerName),
                ExternalSource = sources[coupon.Source],
                Odd = (decimal)odd.DecimalOdds,
                TimeStamp = odd.TimeStamp,
                //ClickThroughURL = odd.ClickThroughURL.ToString()
              };

              this.bookmakerRepository.AddMatchOutcomeOdd(matchOutcomeOdd);
            }
            else
            {
              var persistedOdd = outcomeOdds.First(predicate);
              persistedOdd.Odd = (decimal)odd.DecimalOdds;
            }
          }
        }
        ret.Add(coupon);
      }
      this.fixtureRepository.SaveChanges();

      return ret;
    }

    protected IEnumerable<Match> PersistCoupons(IEnumerable<Model.GenericMatchCoupon> coupons, DateTime couponDate, string tournament)
    {
      var matches = this.fixtureRepository.GetMatchesForOdds(couponDate, tournament);

      var sources = coupons.Select(c => c.Source).Distinct()
        .ToDictionary(s => s, s => this.bookmakerRepository.GetExternalSource(s));

      var bestOddsBookmaker = coupons.Select(c => c.Source).Distinct()
        .ToDictionary(s => s + " Best Available", s => this.bookmakerRepository.FindByName(s + " Best Available"));

      foreach (var coupon in coupons)
      {
        var persistedMatch = matches.FirstOrDefault(m => m.TeamsPlayerA.Slug == coupon.TeamOrPlayerA &&
                                                         m.TeamsPlayerB.Slug == coupon.TeamOrPlayerB);
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
          persistedMatch.MatchCouponURLs.First(u => u.ExternalSource.Source == coupon.Source).MatchCouponURLString = coupon.MatchURL == null ? string.Empty : coupon.MatchURL.ToString();
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

    protected Model.IValueOptions GetValueOptions(DateTime date, string tournamentString, string oddsSourceString, string sportString)
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

  public class FootballOddsService : OddsService, IFootballOddsService
  {
    public FootballOddsService(IFixtureRepository fixtureRepository, IBookmakerRepository bookmakerRepository,
      IStoredProceduresRepository storedProcedureRepository, IPredictionRepository predictionRepository,
      ICouponStrategyProvider couponProvider, IOddsStrategyProvider oddsProvider)
      : base(fixtureRepository, bookmakerRepository, storedProcedureRepository, predictionRepository,
      couponProvider, oddsProvider)
    {
      this.sport = "Football";
    }

    public IEnumerable<FootballFixtureViewModel> FetchAllPreScreenedFootballOdds(DateTime date)
    {
      var matchCoupons = new List<Match>();
      var matchCouponsQualifying = new List<Match>();
      var matchCouponsNotQualifying = new List<Match>();

      var tournaments = DaysTournaments(date, this.sport);
      var oddsSources = this.bookmakerRepository.GetActiveOddsSources()
                                                .OrderByDescending(s => s.PrescreenDecider)
                                                .Select(o => o.Source)
                                                .ToList(); //order by the prescreen decider.  Most likely BestBetting.  
                                                //This is a bit messy but based on what is cached in the text versions of th HTML files

      foreach (var tournament in tournaments.Select(t => t.TournamentName))
      {
        foreach (var source in oddsSources)
        {
          matchCoupons.AddRange(FetchMatchCoupons(date, tournament, source, sport, false, true));
        }
      }

      return Mapper.Map<IEnumerable<Match>, IEnumerable<FootballFixtureViewModel>>(matchCoupons);
    }

    public IEnumerable<FootballCouponViewModel> FetchAllFootballOddsNew(DateTime date)
    {
      var coupons = new List<FootballCouponViewModel>();

      var tournaments = DaysTournaments(date, this.sport);
      var oddsSources = this.bookmakerRepository.GetActiveOddsSources().Select(o => o.Source);

      foreach (var tournament in tournaments.Select(t => t.TournamentName))
      {
        foreach (var source in oddsSources)
        {
          coupons.AddRange(FetchCouponsNew(date, tournament, source, this.sport, true, false));
        }
      }
      return coupons;
    }

    public IEnumerable<FootballFixtureViewModel> FetchAllFootballOdds(DateTime date)
    {
      var matchCoupons = new List<FootballFixtureViewModel>();

      var tournaments = DaysTournaments(date, this.sport);
      var oddsSources = this.bookmakerRepository.GetActiveOddsSources().Select(o => o.Source);

      foreach (var tournament in tournaments.Select(t => t.TournamentName))
      {
        foreach (var source in oddsSources)
        {
          matchCoupons.AddRange(FetchCoupons(date, tournament, source, this.sport, true, false));
        }
      }
      return matchCoupons;
    }

    public IEnumerable<FootballCouponViewModel> FetchCouponsNew(DateTime date, string tournament, string oddsSource, string sport, bool getOdds, bool prescreen)
    {
      var coupons = FetchMatchCouponsNew(date, tournament, oddsSource, sport, getOdds, prescreen);
      return Mapper.Map<IEnumerable<Model.GenericMatchCoupon>, IEnumerable<FootballCouponViewModel>>(coupons);
    }

    public IEnumerable<FootballFixtureViewModel> FetchCoupons(DateTime date, string tournament, string oddsSource, string sport, bool getOdds, bool prescreen)
    {
      var matches = FetchMatchCoupons(date, tournament, oddsSource, sport, getOdds, prescreen);
      return Mapper.Map<IEnumerable<Match>, IEnumerable<FootballFixtureViewModel>>(matches);
    }
  }
}
