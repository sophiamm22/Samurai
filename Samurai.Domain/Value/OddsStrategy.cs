using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jint;

using Samurai.Core;
using Samurai.Domain.Repository;
using Samurai.Domain.Model;
using Samurai.Domain.HtmlElements;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.Domain.Value
{
  public interface IOddsStrategy
  {
    IDictionary<Outcome, IEnumerable<GenericOdd>> GetOdds(IGenericMatchCoupon matchCoupon, DateTime timeStamp);
  }

  public abstract class AbstractOddsStrategy : IOddsStrategy
  {
    protected readonly IWebRepository webRepository;
    protected readonly IBookmakerRepository bookmakerRepository;
    protected readonly IFixtureRepository fixtureRepository;

    public AbstractOddsStrategy(IBookmakerRepository bookmakerRepository, 
      IFixtureRepository fixtureRepository, IWebRepository webRepository)
    {
      if (bookmakerRepository == null) throw new ArgumentNullException("bookmakerRepository");
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (webRepository == null) throw new ArgumentNullException("webRepository");

      this.bookmakerRepository = bookmakerRepository;
      this.fixtureRepository = fixtureRepository;
      this.webRepository = webRepository;
    }
    public abstract IDictionary<Outcome, IEnumerable<GenericOdd>> GetOdds(IGenericMatchCoupon matchCoupon, DateTime timeStamp);
  }

  public class BestBettingOddsStrategy : AbstractOddsStrategy
  {
    public BestBettingOddsStrategy(IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepository webRepository)
      : base(bookmakerRepository, fixtureRepository, webRepository)
    { }

    public override IDictionary<Outcome, IEnumerable<GenericOdd>> GetOdds(IGenericMatchCoupon matchCoupon, DateTime timeStamp)
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
          var currentOutcomeLocal = this.fixtureRepository.GetAlias(((BestBettingOddsCompetitor)oddsToken).Competitor, source, destination);
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
    public OddsCheckerMobiOddsStrategy(IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepository webRepository)
      : base(bookmakerRepository, fixtureRepository, webRepository)
    { }

    public override IDictionary<Outcome, IEnumerable<GenericOdd>> GetOdds(IGenericMatchCoupon matchCoupon, DateTime timeStamp)
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
          var currentOutcomeLocal = this.fixtureRepository.GetAlias(((BestBettingOddsCompetitor)oddsToken).Competitor, source, destination);
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
    public OddsCheckerWebOddsStrategy(IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepository webRepository)
      : base(bookmakerRepository, fixtureRepository, webRepository)
    { }

    public override IDictionary<Outcome, IEnumerable<GenericOdd>> GetOdds(IGenericMatchCoupon matchCoupon, DateTime timeStamp)
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

      var oddsTokens = WebUtils.ParseWebsite<OddsCheckerWebCompetitor, OddsCheckerWebOdds>(
        html, s => Console.WriteLine(s));

      var oddsCheckerJSFile = this.bookmakerRepository.GetOddsCheckerJavaScript();

      var jint = new JintEngine();
      jint.Run(oddsCheckerJSFile);

      var currentOutcome = Outcome.NotAssigned;
      var oddsForOutcome = new List<GenericOdd>();

      foreach (var oddsToken in oddsTokens)
      {
        if (oddsToken is OddsCheckerWebCompetitor)
        {
          var currentOutcomeLocal = this.fixtureRepository.GetAlias(((BestBettingOddsCompetitor)oddsToken).Competitor, source, destination);
          currentOutcome = playerLookup[currentOutcomeLocal];

          oddsForOutcome = new List<GenericOdd>();
          outcomeDictionary.Add(currentOutcome, oddsForOutcome);
        }
        else
        {
          var odd = (OddsCheckerWebOdds)oddsToken;
          var bookmaker = this.bookmakerRepository.FindByOddsCheckerID(odd.OddsCheckerID);
          var bSlip = string.Format("www.oddschecker.com{0}", jint.CallFunction("bSlip", odd.BookmakerID, odd.MarketIDOne, odd.MarketIDTwo, odd.OddsText).ToString());

          oddsForOutcome.Add(new OddsCheckerOdd()
          {
            OddsBeforeCommission = odd.DecimalOdds,
            CommissionPct = (double)(bookmaker.CurrentCommission ?? 0.0m),
            DecimalOdds = odd.DecimalOdds * (1 - (double)(bookmaker.CurrentCommission ?? 0.0m)),
            BookmakerName = bookmaker.BookmakerName,
            Source = "Odds Checker Web",
            BetSlipValue = bSlip,
            TimeStamp = timeStamp,
            Priority = bookmaker.Priority
          });
        }
      }
      return outcomeDictionary;
    }
  }
}
