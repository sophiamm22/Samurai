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

    public void AddTournamentCouponURL(TournamentCouponURLViewModel viewModel)
    {
      var externalSource = this.bookmakerRepository.GetExternalSource(viewModel.ExternalSource);
      var tournament = this.fixtureRepository.GetTournament(viewModel.Tournament);
      this.bookmakerRepository.AddTournamentCouponURL(externalSource, tournament, viewModel.URL);
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
    protected IEnumerable<Model.GenericMatchCoupon> FetchMatchCoupons(DateTime date, string tournament, string oddsSource, string sport, bool getOdds, bool preScreenOdds)
    {
      var valueOptions = GetValueOptions(date, tournament, oddsSource, sport);
      var couponStrategy = this.couponProvider.CreateCouponStrategy(valueOptions);
      var tournamentEvent = this.fixtureRepository.GetTournamentEventFromTournamentAndDate(date, tournament);

      var coupons = 
        couponStrategy.GetMatches()
                      .ToList();

      coupons.ForEach(x => x.TournamentEventName = tournamentEvent.EventName);

      var todaysCoupons = GetTodaysCoupons(date, tournamentEvent, coupons);
      if (coupons.Count == 0) 
        return Enumerable.Empty<Model.GenericMatchCoupon>(); //get out ASAP

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
          if (!coupon.InPlay)
            coupon.ActualOdds = oddsStrategy.GetOdds(coupon, valueOptions.CouponDate, timeStamp);
        }
      }
      else
      {
        dontGetOddsFor = todaysCoupons;
      }

      if (valueOptions.OddsSource.PrescreenDecider)
        this.prescreenedCouponTarget.AddRange(getOddsFor);

      var matchesOdds = getOddsFor == null || getOddsFor.Count() == 0 ? Enumerable.Empty<Model.GenericMatchCoupon>() : PersistCoupons(getOddsFor, date, tournament);
      var matchesNoOdds = dontGetOddsFor == null || dontGetOddsFor.Count() == 0 ? Enumerable.Empty<Model.GenericMatchCoupon>() : PersistCoupons(dontGetOddsFor, date, tournament);

      var matches = new List<Model.GenericMatchCoupon>();
      matches.AddRange(matchesOdds);
      matches.AddRange(matchesNoOdds);
      return matches;
    }

    private List<Model.GenericMatchCoupon> GetTodaysCoupons(DateTime date, TournamentEvent tournamentEvent, IEnumerable<Model.GenericMatchCoupon> coupons)
    {

      var couponsDic = new Dictionary<string, Model.GenericMatchCoupon>();

      foreach (var coupon in coupons)
      {
        var key = string.Empty;

        if (string.IsNullOrEmpty(coupon.FirstNameA) && string.IsNullOrEmpty(coupon.FirstNameB))
          key = string.Format("{0}|{1}", coupon.TeamOrPlayerA, coupon.TeamOrPlayerB);
        else
          key = string.Format("{0},{1}|{2},{3}", coupon.TeamOrPlayerA, coupon.FirstNameA, coupon.TeamOrPlayerB, coupon.FirstNameB);

        if (!couponsDic.ContainsKey(key))
          couponsDic.Add(key, coupon);
      }

      var tournamentEventID = tournamentEvent.Id;

      var persistedFixtures =
        this.fixtureRepository
            .GetDaysMatchesWithTeamsTournaments(date, sport)
            .Where(x => x.TournamentEventID == tournamentEventID)
            .ToList();
      var todaysCoupons = new List<Model.GenericMatchCoupon>();

      foreach (var pf in persistedFixtures)
      {
        var lookup = string.Empty;
        if (string.IsNullOrEmpty(pf.TeamsPlayerA.FirstName) && string.IsNullOrEmpty(pf.TeamsPlayerB.FirstName))
          lookup = string.Format("{0}|{1}", pf.TeamsPlayerA.Name, pf.TeamsPlayerB.Name);
        else
          lookup = string.Format("{0},{1}|{2},{3}", pf.TeamsPlayerA.Name, pf.TeamsPlayerA.FirstName, pf.TeamsPlayerB.Name, pf.TeamsPlayerB.FirstName);
        if (couponsDic.ContainsKey(lookup))
          todaysCoupons.Add(couponsDic[lookup]);
      }
      return todaysCoupons;
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

    protected IEnumerable<Model.GenericMatchCoupon> FetchOddsForMatches(Model.IValueOptions valueOptions, IEnumerable<Match> matches)
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
          MatchURL = new Uri(match.MatchCouponURLs.First(m => m.ExternalSource.Source == valueOptions.OddsSource.Source).MatchCouponURLString),
          InPlay = match.InPlay
        };
        coupon.ActualOdds = oddsStrategy.GetOdds(coupon, valueOptions.CouponDate, timeStamp);
        coupons.Add(coupon);
      }
      var matchesReturn = PersistCoupons(coupons, valueOptions.CouponDate, valueOptions.Tournament.TournamentName);
      return coupons;
    }

    protected IEnumerable<Model.GenericMatchCoupon> PersistCoupons(IEnumerable<Model.GenericMatchCoupon> coupons, DateTime couponDate, string tournament)
    {
      var ret = new List<Model.GenericMatchCoupon>();
      var matches = this.fixtureRepository.GetMatchesForTournament(couponDate, tournament);

      var sources = coupons.Select(c => c.Source).Distinct()
        .ToDictionary(s => s, s => this.bookmakerRepository.GetExternalSource(s));

      var bestOddsBookmaker = coupons.Select(c => c.Source).Distinct()
        .ToDictionary(s => s + " Best Available", s => this.bookmakerRepository.FindByName(s + " Best Available"));

      foreach (var coupon in coupons)
      {
        var retCoupon = Mapper.Map<Model.GenericMatchCoupon, Model.GenericMatchCoupon>(coupon);

        var teamPlayerA = this.fixtureRepository.GetTeamOrPlayerFromNameAndMaybeFirstName(coupon.TeamOrPlayerA, coupon.FirstNameA);
        var teamPlayerB = this.fixtureRepository.GetTeamOrPlayerFromNameAndMaybeFirstName(coupon.TeamOrPlayerB, coupon.FirstNameB);
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

          Func<MatchOutcomeOdd, Model.Outcome, bool> predicate =
            (o, oc) => o.ExternalSource.Source == coupon.Source &&
                       o.Bookmaker.BookmakerName == bestAvailableBookmaker &&
                       o.TimeStamp == outcomeOdds.Where(o2 => o.ExternalSource == o2.ExternalSource && o.Bookmaker == o2.Bookmaker)
                                                 .Select(x=>x.TimeStamp)
                                                 .DefaultIfEmpty(DateTime.MinValue)
                                                 .Max() &&
                       o.Odd == (decimal)coupon.HeadlineOdds[oc];

          var persisistedOdd = outcomeOdds.FirstOrDefault(x => predicate(x, outcome));

          if (persisistedOdd == null)
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
            retCoupon.HeadlineOdds.Add(outcome, coupon.HeadlineOdds[outcome]);
          }
          else
          {
            if (!retCoupon.HeadlineOdds.ContainsKey(outcome))
              retCoupon.HeadlineOdds.Add(outcome, (double)persisistedOdd.Odd);//dubious
          }
        }

        foreach (var outcome in coupon.ActualOdds.Keys)
        {
          var probForOutcome = outcomeProbs.First(p => p.MatchOutcomeID == (int)outcome);

          var outcomeOdds = this.bookmakerRepository
                                 .GetMatchOutcomeOdds(probForOutcome.Id)
                                 .ToList();

          var newActualOdds = new List<Model.GenericOdd>();
          foreach (var odd in coupon.ActualOdds[outcome])
          {
            Func<MatchOutcomeOdd, Model.Outcome, bool> predicate =
              (o, oc) => o.ExternalSource.Source == coupon.Source &&
                        (o.Bookmaker == null || o.Bookmaker.BookmakerName == odd.BookmakerName) &&
                         o.TimeStamp == outcomeOdds.Where(o2 => o.ExternalSource == o2.ExternalSource && (o.Bookmaker == null || o.Bookmaker == o2.Bookmaker))
                                                   .Select(x => x.TimeStamp)
                                                   .DefaultIfEmpty(DateTime.MinValue)
                                                   .Max() &&
                         o.Odd == (decimal)odd.DecimalOdds;

            var persisistedOdd = outcomeOdds.FirstOrDefault(x => predicate(x, outcome));

            if (persisistedOdd == null)
            {
              var matchOutcomeOdd = new MatchOutcomeOdd()
              {
                MatchOutcomeProbabilitiesInMatchID = probForOutcome.Id,
                Bookmaker = this.bookmakerRepository.FindByName(odd.BookmakerName),
                ExternalSource = sources[coupon.Source],
                Odd = (decimal)odd.DecimalOdds,
                TimeStamp = odd.TimeStamp,
                ClickThroughURL = odd.ClickThroughURL == null ? null : odd.ClickThroughURL.ToString()
              };

              this.bookmakerRepository.AddMatchOutcomeOdd(matchOutcomeOdd);
              if (!retCoupon.ActualOdds.ContainsKey(outcome))
                retCoupon.ActualOdds.Add(outcome, new List<Model.GenericOdd>());
              newActualOdds.Add(odd);
            }
            else
            {
              if (!retCoupon.ActualOdds.ContainsKey(outcome))
                retCoupon.ActualOdds.Add(outcome, new List<Model.GenericOdd>());

              var equivalentOdd = Mapper.Map<Model.GenericOdd, Model.GenericOdd>(odd);
              
              equivalentOdd.TimeStamp = persisistedOdd.TimeStamp;
              equivalentOdd.OddsBeforeCommission = (double)persisistedOdd.Odd;

              newActualOdds.Add(equivalentOdd);
            }
          }
          retCoupon.ActualOdds[outcome] = newActualOdds;
        }
        ret.Add(retCoupon);
      }
      this.fixtureRepository.SaveChanges();

      return ret;
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

    public IEnumerable<FootballCouponViewModel> FetchAllPreScreenedFootballOdds(DateTime date)
    {
      var matchCoupons = new List<FootballCouponViewModel>();

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
          matchCoupons.AddRange(FetchCoupons(date, tournament, source, sport, false, true));
        }
      }

      return matchCoupons;
    }

    public IEnumerable<FootballCouponViewModel> FetchAllFootballOdds(DateTime date)
    {
      var coupons = new List<FootballCouponViewModel>();

      var tournaments = DaysTournaments(date, this.sport);
      var oddsSources = this.bookmakerRepository.GetActiveOddsSources().Select(o => o.Source);

      foreach (var tournament in tournaments.Select(t => t.TournamentName))
      {
        foreach (var source in oddsSources)
        {
          coupons.AddRange(FetchCoupons(date, tournament, source, this.sport, true, false));
        }
      }
      return coupons;
    }

    public IEnumerable<FootballCouponViewModel> FetchCoupons(DateTime date, string tournament, string oddsSource, string sport, bool getOdds, bool prescreen)
    {
      var coupons = FetchMatchCoupons(date, tournament, oddsSource, sport, getOdds, prescreen);
      return Mapper.Map<IEnumerable<Model.GenericMatchCoupon>, IEnumerable<FootballCouponViewModel>>(coupons);
    }
  }
}
