using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Model;
using Samurai.Domain.Entities;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Core;
using Samurai.Domain.Exceptions;
using Samurai.Domain.HtmlElements;

namespace Samurai.Domain.Value.Async
{
  public interface IAsyncOddsStrategy
  {
    Task<IDictionary<Outcome, IEnumerable<GenericOdd>>> GetOdds(GenericMatchCoupon matchCoupon, DateTime couponDate, DateTime timeStamp);
  }

  public abstract class AbstractAsyncOddsStrategy : IAsyncOddsStrategy
  {
    protected readonly Sport sport;
    protected readonly IWebRepositoryProviderAsync webRepositoryProvider;
    protected readonly IBookmakerRepository bookmakerRepository;
    protected readonly IFixtureRepository fixtureRepository;

    public AbstractAsyncOddsStrategy(Sport sport, IBookmakerRepository bookmakerRepository, 
      IFixtureRepository fixtureRepository, IWebRepositoryProviderAsync webRepositoryProvider)
    {
      if (sport == null) throw new ArgumentNullException("sport");
      if (bookmakerRepository == null) throw new ArgumentNullException("bookmakerRepository");
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (webRepositoryProvider == null) throw new ArgumentNullException("webRepository");

      this.sport = sport;
      this.bookmakerRepository = bookmakerRepository;
      this.fixtureRepository = fixtureRepository;
      this.webRepositoryProvider = webRepositoryProvider;
    }
    public abstract Task<IDictionary<Outcome, IEnumerable<GenericOdd>>> GetOdds(GenericMatchCoupon matchCoupon, DateTime couponDate, DateTime timeStamp);
  }

  public class BestBettingAsyncOddsStrategy : AbstractAsyncOddsStrategy
  {
    public BestBettingAsyncOddsStrategy(Sport sport, IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepositoryProviderAsync webRepositoryProvider)
      : base(sport, bookmakerRepository, fixtureRepository, webRepositoryProvider)
    { }

    public override async Task<IDictionary<Outcome, IEnumerable<GenericOdd>>> GetOdds(GenericMatchCoupon matchCoupon, DateTime couponDate, DateTime timeStamp)
    {
      var playerLookup = new Dictionary<string, Outcome>()
      {
        { matchCoupon.TeamOrPlayerA, Outcome.HomeWin },
        { "Draw", Outcome.Draw },
        { matchCoupon.TeamOrPlayerB, Outcome.AwayWin }
      };

      var source = 
        this.fixtureRepository
            .GetExternalSource("Best Betting");
      var destination = 
        this.fixtureRepository
            .GetExternalSource("Value Samurai");

      var outcomeDictionary = new Dictionary<Outcome, IEnumerable<GenericOdd>>();

      var webRepository = 
        this.webRepositoryProvider
            .CreateWebRepository(couponDate);

      var oddsHTML = await webRepository.GetHTML(matchCoupon.MatchURL);
      var oddsTokens =
        WebUtils.ParseWebsite<BestBettingOddsCompetitor, BestBettingOdds>(
        oddsHTML, s => { });

      var currentOutcome = Outcome.NotAssigned;
      var oddsForOutcome = new List<GenericOdd>();
      var missingBookmakerAlias = new List<MissingBookmakerAlias>();

      foreach (var oddsToken in oddsTokens)
      {
        if (oddsToken is BestBettingOddsCompetitor)
        {
          var competitor = ((BestBettingOddsCompetitor)oddsToken).Competitor;

          var currentOutcomeLocal = competitor == "Draw" ? null : this.fixtureRepository.GetAlias(((BestBettingOddsCompetitor)oddsToken).Competitor, source, destination, this.sport);
          currentOutcome = playerLookup[competitor == "Draw" ? competitor : currentOutcomeLocal.Name];

          oddsForOutcome = new List<GenericOdd>();
          outcomeDictionary.Add(currentOutcome, oddsForOutcome);
        }
        else
        {
          var odd = (BestBettingOdds)oddsToken;
          var bookmakerName = this.bookmakerRepository.GetAlias(odd.Bookmaker, source, destination);
          var bookmaker = this.bookmakerRepository.FindByName(bookmakerName);
          if (bookmaker == null)
          {
            missingBookmakerAlias.Add(new MissingBookmakerAlias
            {
              Bookmaker = odd.Bookmaker,
              ExternalSource = source.Source
            });
            continue;
          }

          oddsForOutcome.Add(new BestBettingOdd()
          {
            OddsBeforeCommission = odd.DecimalOdds,
            CommissionPct = (double)(bookmaker.CurrentCommission ?? 0.0m),
            DecimalOdds = odd.DecimalOdds * (1 - (double)(bookmaker.CurrentCommission ?? 0.0m)),
            BookmakerName = bookmaker.BookmakerName,
            Source = "Best Betting",
            TimeStamp = timeStamp,
            Priority = bookmaker.Priority,
            ClickThroughURL = odd.ClickThroughURL
          });
        }
      }
      if (missingBookmakerAlias.Count() != 0)
        throw new MissingBookmakerAliasException(missingBookmakerAlias, "Missing bookmaker alias");

      return outcomeDictionary;
    }
  }

  public class OddsCheckerMobiAsyncOddsStrategy : AbstractAsyncOddsStrategy
  {
    public OddsCheckerMobiAsyncOddsStrategy(Sport sport, IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepositoryProviderAsync webRepositoryProvider)
      : base(sport, bookmakerRepository, fixtureRepository, webRepositoryProvider)
    { }

    public override async Task<IDictionary<Outcome, IEnumerable<GenericOdd>>> GetOdds(GenericMatchCoupon matchCoupon, DateTime couponDate, DateTime timeStamp)
    {
      var playerLookup = new Dictionary<string, Outcome>()
      {
        { matchCoupon.TeamOrPlayerA, Outcome.HomeWin },
        { "Draw", Outcome.Draw },
        { matchCoupon.TeamOrPlayerB, Outcome.AwayWin }
      };

      var source = 
        this.fixtureRepository
            .GetExternalSource("Odds Checker Mobi");
      var destination = 
        this.fixtureRepository
            .GetExternalSource("Value Samurai");

      var outcomeDictionary = new Dictionary<Outcome, IEnumerable<GenericOdd>>();

      var webRepository = 
        this.webRepositoryProvider
            .CreateWebRepository(couponDate);

      var html = await webRepository.GetHTML(matchCoupon.MatchURL);

      var oddsTokens =
        WebUtils.ParseWebsite<OddsCheckerMobiCompetitor, OddsCheckerMobiOdds>(html, s => { });

      var currentOutcome = Outcome.NotAssigned;
      var oddsForOutcome = new List<GenericOdd>();
      var missingBookmakerAlias = new List<MissingBookmakerAlias>();

      foreach (var oddsToken in oddsTokens)
      {
        if (oddsToken is OddsCheckerMobiCompetitor)
        {
          var competitor = ((OddsCheckerMobiCompetitor)oddsToken).Outcome;
          var currentOutcomeLocal = competitor == "Draw" ? null : this.fixtureRepository.GetAlias(competitor, source, destination, sport);
          currentOutcome = playerLookup[competitor == "Draw" ? competitor : currentOutcomeLocal.Name];

          oddsForOutcome = new List<GenericOdd>();
          outcomeDictionary.Add(currentOutcome, oddsForOutcome);
        }
        else
        {
          var odd = (OddsCheckerMobiOdds)oddsToken;
          var bookmakerName = this.bookmakerRepository.GetAlias(odd.Bookmaker, source, destination);
          var bookmaker = this.bookmakerRepository.FindByName(bookmakerName);
          if (bookmaker == null)
          {
            missingBookmakerAlias.Add(new MissingBookmakerAlias
            {
              Bookmaker = odd.Bookmaker,
              ExternalSource = source.Source
            });
            continue;
          }

          oddsForOutcome.Add(new OddsCheckerOdd()
          {
            OddsBeforeCommission = odd.DecimalOdds,
            CommissionPct = (double)(bookmaker.CurrentCommission ?? 0.0m),
            DecimalOdds = odd.DecimalOdds * (1 - (double)(bookmaker.CurrentCommission ?? 0.0m)),
            BookmakerName = bookmaker.BookmakerName,
            Source = "Odds Checker Mobi",
            BetSlipValue = odd.BetSlipValue,
            TimeStamp = timeStamp,
            Priority = bookmaker.Priority
          });
        }
      }
      if (missingBookmakerAlias.Count() != 0)
        throw new MissingBookmakerAliasException(missingBookmakerAlias, "Missing bookmaker alias");

      return outcomeDictionary;
    }
  }

  public class OddsCheckerWebAsyncOddsStrategy : AbstractAsyncOddsStrategy
  {
    public OddsCheckerWebAsyncOddsStrategy(Sport sport, IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepositoryProviderAsync webRepositoryProvider)
      : base(sport, bookmakerRepository, fixtureRepository, webRepositoryProvider)
    { }

    public override async Task<IDictionary<Outcome, IEnumerable<GenericOdd>>> GetOdds(GenericMatchCoupon matchCoupon, DateTime couponDate, DateTime timeStamp)
    {
      var playerLookup = new Dictionary<string, Outcome>()
      {
        { matchCoupon.TeamOrPlayerA, Outcome.HomeWin },
        { "Draw", Outcome.Draw },
        { matchCoupon.TeamOrPlayerB, Outcome.AwayWin }
      };

      var source = 
        this.fixtureRepository
            .GetExternalSource("Odds Checker Web");

      var destination = 
        this.fixtureRepository
            .GetExternalSource("Value Samurai");

      var outcomeDictionary = new Dictionary<Outcome, IEnumerable<GenericOdd>>();

      var webRepository = 
        this.webRepositoryProvider
            .CreateWebRepository(couponDate);

      var html = await webRepository.GetHTML(matchCoupon.MatchURL);

      var oddsTokens = new List<IRegexableWebsite>();

      oddsTokens.AddRange(WebUtils.ParseWebsite<OddsCheckerWebMarketID, OddsCheckerWebCard>(html, s => { }));
      oddsTokens.AddRange(WebUtils.ParseWebsite<OddsCheckerWebCompetitor, OddsCheckerWebOdds>(html, s => { }));

      #region deprecated
      //deprecated - redirection used to be handled by javascript.  We now get an easy to hack URL
      //var oddsCheckerJSFile = this.bookmakerRepository.GetOddsCheckerJavaScript();

      //var jint = new JintEngine();
      //jint.Run(oddsCheckerJSFile);
      #endregion

      string webCard = ((OddsCheckerWebCard)oddsTokens.First(o => o is OddsCheckerWebCard)).CardID;
      string webMarketID = ((OddsCheckerWebMarketID)oddsTokens.First(o => o is OddsCheckerWebMarketID)).MarketID;

      var bestBookies =
        (from odd in oddsTokens.OfType<OddsCheckerWebOdds>()
         group odd by odd.OddsCheckerID into groupedOdds
         select new
         {
           ID = groupedOdds.Key,
           BestBookies = groupedOdds.Where(x => x.IsBestOdd).Aggregate(string.Empty, (acc, item) => acc + "," + item.BookmakerID)
         })
        .ToDictionary(x => x.ID, x => x.BestBookies);

      var currentOutcome = Outcome.NotAssigned;
      var oddsForOutcome = new List<GenericOdd>();
      var missingBookmakerAlias = new List<MissingBookmakerAlias>();

      foreach (var oddsToken in oddsTokens)
      {
        if (oddsToken is OddsCheckerWebCompetitor)
        {
          var competitor = ((OddsCheckerWebCompetitor)oddsToken).Outcome;
          var currentOutcomeLocal = competitor == "Draw" ? null : this.fixtureRepository.GetAlias(competitor, source, destination, sport);
          currentOutcome = playerLookup[competitor == "Draw" ? competitor : currentOutcomeLocal.Name];

          oddsForOutcome = new List<GenericOdd>();
          outcomeDictionary.Add(currentOutcome, oddsForOutcome);
        }
        else if (oddsToken is OddsCheckerWebOdds)
        {
          var odd = (OddsCheckerWebOdds)oddsToken;
          if (odd.BookmakerID == "SI")
            continue;
          var bookmaker = this.bookmakerRepository.FindByOddsCheckerID(odd.BookmakerID);
          if (bookmaker == null)
          {
            missingBookmakerAlias.Add(new MissingBookmakerAlias
            {
              Bookmaker = odd.BookmakerID,
              ExternalSource = source.Source
            });
            continue;
          }

          var clickThroughURL = string.Format("http://www.oddschecker.com/betslip?bk={0}&mkid={1}&pid={2}&cardId={3}&bestBookies={4}",
            odd.BookmakerID, webMarketID, odd.OddsCheckerID, webCard, bestBookies[odd.OddsCheckerID]);
          //var bSlip = string.Format("www.oddschecker.com{0}", jint.CallFunction("bSlip", odd.BookmakerID, odd.MarketIDOne, odd.MarketIDTwo, odd.OddsText).ToString());

          oddsForOutcome.Add(new OddsCheckerOdd()
          {
            OddsBeforeCommission = odd.DecimalOdds,
            CommissionPct = (double)(bookmaker.CurrentCommission ?? 0.0m),
            DecimalOdds = odd.DecimalOdds * (1 - (double)(bookmaker.CurrentCommission ?? 0.0m)),
            BookmakerName = bookmaker.BookmakerName,
            Source = "Odds Checker Web",
            ClickThroughURL = new Uri(clickThroughURL),
            TimeStamp = timeStamp,
            Priority = bookmaker.Priority
          });
        }
      }
      if (missingBookmakerAlias.Count() != 0)
        throw new MissingBookmakerAliasException(missingBookmakerAlias, "Missing bookmaker alias");

      return outcomeDictionary;
    }
  }



}
