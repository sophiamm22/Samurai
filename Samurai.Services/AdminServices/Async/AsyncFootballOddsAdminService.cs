using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Services.Contracts.Async;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Domain.Value.Async;
using Model = Samurai.Domain.Model;
using Samurai.Web.ViewModels.Value;

namespace Samurai.Services.AdminServices.Async
{
  public abstract class AsyncOddsAdminService : IAsyncOddsAdminService
  {
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly IBookmakerRepository bookmakerRepository;
    protected readonly IStoredProceduresRepository storedProcedureRepository;
    protected readonly IPredictionRepository predicitonRepository;
    protected readonly IAsyncCouponStrategyProvider couponProvider;
    protected readonly IAsyncOddsStrategyProvider oddsProvider;

    protected string sport;

    public AsyncOddsAdminService(IFixtureRepository fixtureRepository, IBookmakerRepository bookmakerRepository,
      IStoredProceduresRepository storedProcedureRepository, IPredictionRepository predicitonRepository,
      IAsyncCouponStrategyProvider couponProvider, IAsyncOddsStrategyProvider oddsProvider)
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

    protected async Task<IEnumerable<Model.GenericMatchCoupon>> FetchMatchCoupons(DateTime date, string tournament, string oddsSource, string sport)
    {
      var valueOptions = GetValueOptions(date, tournament, oddsSource, sport);
      
      var couponStrategy = 
        this.couponProvider
            .CreateCouponStrategy(valueOptions);

      var tournamentEvent = 
        this.fixtureRepository
            .GetTournamentEventFromTournamentAndDate(date, tournament);

      var coupons = (await couponStrategy.GetMatches()).ToList();

      coupons.ForEach(x => x.TournamentEventName = tournamentEvent.EventName);

      var todaysCoupons = GetTodaysCoupons(date, tournamentEvent, coupons);

      if (coupons.Count == 0)
        return Enumerable.Empty<Model.GenericMatchCoupon>(); //get out ASAP

      var oddsStrategy = 
        this.oddsProvider
            .CreateOddsStrategy(valueOptions);

      var timeStamp = DateTime.Now;
      foreach (var coupon in todaysCoupons)
      {
        if (!coupon.InPlay)
          coupon.ActualOdds = await oddsStrategy.GetOdds(coupon, valueOptions.CouponDate, timeStamp);
      }

      var matchesOdds = (todaysCoupons == null || todaysCoupons.Count() == 0) ? 
                          Enumerable.Empty<Model.GenericMatchCoupon>() :
                          await PersistCoupons(todaysCoupons, date, tournament);

      var matches = new List<Model.GenericMatchCoupon>();
      matches.AddRange(matchesOdds);
      return matches;
    }

    protected async Task<IEnumerable<Model.GenericMatchCoupon>> PersistCoupons(IEnumerable<Model.GenericMatchCoupon> coupons, DateTime couponDate, string tournament)
    {
      var ret = new List<Model.GenericMatchCoupon>();
      var matches = 
        this.fixtureRepository
            .GetMatchesForTournament(couponDate, tournament);

      var sources = 
        coupons.Select(c => c.Source)
               .Distinct()
               .ToDictionary(s => s, s => this.bookmakerRepository.GetExternalSource(s));

      var bestOddsBookmaker = 
        coupons.Select(c => c.Source)
               .Distinct()
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
                                                 .Select(x => x.TimeStamp)
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
      await Task.Run(() => this.fixtureRepository.SaveChanges());

      return ret;
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
  
  public class AsyncFootballOddsAdminService : AsyncOddsAdminService, IAsyncFootballOddsAdminService
  {
    public AsyncFootballOddsAdminService(IFixtureRepository fixtureRepository, IBookmakerRepository bookmakerRepository,
      IStoredProceduresRepository storedProcedureRepository, IPredictionRepository predictionRepository,
      IAsyncCouponStrategyProvider couponProvider, IAsyncOddsStrategyProvider oddsProvider)
      : base(fixtureRepository, bookmakerRepository, storedProcedureRepository, predictionRepository,
      couponProvider, oddsProvider)
    {
      this.sport = "Football";
    }

    public async Task<IEnumerable<FootballCouponViewModel>> FetchFootballOddsForTournamentSource(
      DateTime date, TournamentViewModel tournament, OddsSourceViewModel oddsSource)
    {
      return await FetchCoupons(date, tournament.TournamentName, oddsSource.Source);
    }

    public async Task<IEnumerable<FootballCouponViewModel>> FetchAllFootballOdds(DateTime date)
    {
      var coupons = new List<FootballCouponViewModel>();

      var tournaments = DaysTournaments(date, this.sport).ToList();
      var oddsSources = this.bookmakerRepository.GetActiveOddsSources().ToList();

      foreach (var tournament in tournaments)
      {
        foreach (var source in oddsSources)
        {
          var tournamentViewModel = new TournamentViewModel { TournamentName = tournament.TournamentName };
          var oddsSourceViewModel = new OddsSourceViewModel { Source = source.Source };
          coupons.AddRange(await FetchFootballOddsForTournamentSource(date, tournamentViewModel, oddsSourceViewModel));
        }
      }
      return coupons;
    }

    public async Task<IEnumerable<FootballCouponViewModel>> FetchCoupons(DateTime date, string tournament, string oddsSource)
    {
      var coupons = await FetchMatchCoupons(date, tournament, oddsSource, this.sport);
      return Mapper.Map<IEnumerable<Model.GenericMatchCoupon>, IEnumerable<FootballCouponViewModel>>(coupons);
    }
  }
}
