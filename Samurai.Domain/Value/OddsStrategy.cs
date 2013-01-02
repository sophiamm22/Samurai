using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jint;

using Samurai.Core;
using Samurai.Domain.Repository;
using Samurai.Domain.Model;
using Samurai.Domain.HtmlElements;
using Samurai.Domain.Entities;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.Domain.Value
{
  public interface IOddsStrategy
  {
    IDictionary<Outcome, IEnumerable<GenericOdd>> GetOdds(GenericMatchCoupon matchCoupon, DateTime timeStamp);
  }

  public abstract class AbstractOddsStrategy : IOddsStrategy
  {
    protected readonly Sport sport;
    protected readonly IWebRepository webRepository;
    protected readonly IBookmakerRepository bookmakerRepository;
    protected readonly IFixtureRepository fixtureRepository;

    public AbstractOddsStrategy(Sport sport, IBookmakerRepository bookmakerRepository, 
      IFixtureRepository fixtureRepository, IWebRepository webRepository)
    {
      if (sport == null) throw new ArgumentNullException("sport");
      if (bookmakerRepository == null) throw new ArgumentNullException("bookmakerRepository");
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (webRepository == null) throw new ArgumentNullException("webRepository");

      this.sport = sport;
      this.bookmakerRepository = bookmakerRepository;
      this.fixtureRepository = fixtureRepository;
      this.webRepository = webRepository;
    }
    public abstract IDictionary<Outcome, IEnumerable<GenericOdd>> GetOdds(GenericMatchCoupon matchCoupon, DateTime timeStamp);
  }

  public class BestBettingOddsStrategy : AbstractOddsStrategy
  {
    public BestBettingOddsStrategy(Sport sport, IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepository webRepository)
      : base(sport, bookmakerRepository, fixtureRepository, webRepository)
    { }

    public override IDictionary<Outcome, IEnumerable<GenericOdd>> GetOdds(GenericMatchCoupon matchCoupon, DateTime timeStamp)
    {
      var playerLookup = new Dictionary<string, Outcome>()
      {
        { matchCoupon.TeamOrPlayerA, Outcome.HomeWin },
        { "Draw", Outcome.Draw },
        { matchCoupon.TeamOrPlayerB, Outcome.AwayWin }
      };

      var source = this.fixtureRepository.GetExternalSource("Best Betting");
      var destination = this.fixtureRepository.GetExternalSource("Value Samurai");

      var outcomeDictionary = new Dictionary<Outcome, IEnumerable<GenericOdd>>();
      var oddsHTML = webRepository.GetHTML(new Uri[] { matchCoupon.MatchURL }, s => Console.WriteLine(s)).First();
      var oddsTokens = WebUtils.ParseWebsite<BestBettingOddsCompetitor, BestBettingOdds>(
        oddsHTML, s => Console.WriteLine(s));

      var currentOutcome = Outcome.NotAssigned;
      var oddsForOutcome = new List<GenericOdd>();

      foreach (var oddsToken in oddsTokens)
      {
        if (oddsToken is BestBettingOddsCompetitor)
        {
          var currentOutcomeLocal = this.fixtureRepository.GetAlias(((BestBettingOddsCompetitor)oddsToken).Competitor, source, destination, this.sport);
          currentOutcome = playerLookup[currentOutcomeLocal];

          oddsForOutcome = new List<GenericOdd>();
          outcomeDictionary.Add(currentOutcome, oddsForOutcome);
        }
        else
        {
          var odd = (BestBettingOdds)oddsToken;
          var bookmakerName = this.bookmakerRepository.GetAlias(odd.Bookmaker, source, destination);
          var bookmaker = this.bookmakerRepository.FindByName(bookmakerName);

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
      return outcomeDictionary;
    }
  }

  public class OddsCheckerMobiOddsStrategy : AbstractOddsStrategy
  {
    public OddsCheckerMobiOddsStrategy(Sport sport, IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepository webRepository)
      : base(sport, bookmakerRepository, fixtureRepository, webRepository)
    { }

    public override IDictionary<Outcome, IEnumerable<GenericOdd>> GetOdds(GenericMatchCoupon matchCoupon, DateTime timeStamp)
    {
      var playerLookup = new Dictionary<string, Outcome>()
      {
        { matchCoupon.TeamOrPlayerA, Outcome.HomeWin },
        { "Draw", Outcome.Draw },
        { matchCoupon.TeamOrPlayerB, Outcome.AwayWin }
      };

      var source = this.fixtureRepository.GetExternalSource("Odds Checker Mobi");
      var destination = this.fixtureRepository.GetExternalSource("Value Samurai");

      var outcomeDictionary = new Dictionary<Outcome, IEnumerable<GenericOdd>>();

      var html = webRepository.GetHTML(new[] { matchCoupon.MatchURL }, s => Console.WriteLine(s), matchCoupon.MatchURL.ToString())
                               .First();

      var oddsTokens = WebUtils.ParseWebsite<OddsCheckerMobiCompetitor, OddsCheckerMobiOdds>(
        html, s => Console.WriteLine(s));

      var currentOutcome = Outcome.NotAssigned;
      var oddsForOutcome = new List<GenericOdd>();

      foreach (var oddsToken in oddsTokens)
      {
        if (oddsToken is OddsCheckerMobiCompetitor)
        {
          var currentOutcomeLocal = this.fixtureRepository.GetAlias(((OddsCheckerMobiCompetitor)oddsToken).Outcome, source, destination, sport);
          currentOutcome = playerLookup[currentOutcomeLocal];

          oddsForOutcome = new List<GenericOdd>();
          outcomeDictionary.Add(currentOutcome, oddsForOutcome);
        }
        else
        {
          var odd = (OddsCheckerMobiOdds)oddsToken;
          var bookmakerName = this.bookmakerRepository.GetAlias(odd.Bookmaker, source, destination);
          var bookmaker = this.bookmakerRepository.FindByName(bookmakerName);

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
      return outcomeDictionary;
    }
  }

  public class OddsCheckerWebOddsStrategy : AbstractOddsStrategy
  {
    public OddsCheckerWebOddsStrategy(Sport sport, IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepository webRepository)
      : base(sport, bookmakerRepository, fixtureRepository, webRepository)
    { }

    public override IDictionary<Outcome, IEnumerable<GenericOdd>> GetOdds(GenericMatchCoupon matchCoupon, DateTime timeStamp)
    {
      var playerLookup = new Dictionary<string, Outcome>()
      {
        { matchCoupon.TeamOrPlayerA, Outcome.HomeWin },
        { "Draw", Outcome.Draw },
        { matchCoupon.TeamOrPlayerB, Outcome.AwayWin }
      };

      var source = this.fixtureRepository.GetExternalSource("Odds Checker Web");
      var destination = this.fixtureRepository.GetExternalSource("Value Samurai");

      var outcomeDictionary = new Dictionary<Outcome, IEnumerable<GenericOdd>>();

      var html = webRepository.GetHTML(new[] { matchCoupon.MatchURL }, s => Console.WriteLine(s), matchCoupon.MatchURL.ToString())
                               .First();

      var oddsTokens = new List<IRegexableWebsite>();

      oddsTokens.AddRange(WebUtils.ParseWebsite<OddsCheckerWebMarketID, OddsCheckerWebCard>(html, s => Console.WriteLine(s)));
      oddsTokens.AddRange(WebUtils.ParseWebsite<OddsCheckerWebCompetitor, OddsCheckerWebOdds>(html, s => Console.WriteLine(s)));

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

      foreach (var oddsToken in oddsTokens)
      {
        if (oddsToken is OddsCheckerWebCompetitor)
        {
          var currentOutcomeLocal = this.fixtureRepository.GetAlias(((OddsCheckerWebCompetitor)oddsToken).Outcome, source, destination, sport);
          currentOutcome = playerLookup[currentOutcomeLocal];

          oddsForOutcome = new List<GenericOdd>();
          outcomeDictionary.Add(currentOutcome, oddsForOutcome);
        }
        else if (oddsToken is OddsCheckerWebOdds)
        {
          var odd = (OddsCheckerWebOdds)oddsToken;
          if (odd.BookmakerID == "SI")
            continue;
          var bookmaker = this.bookmakerRepository.FindByOddsCheckerID(odd.BookmakerID);
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
      return outcomeDictionary;
    }
  }
}
