using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jint;

using Samurai.Core;
using Samurai.Domain.Repository;
using Samurai.Domain.Model;
using Samurai.Domain.HtmlElements;

namespace Samurai.Domain.Value
{
  public abstract class AbstractOddsStrategy
  {
    protected IWebRepository _webRepository;
    public AbstractOddsStrategy(IWebRepository webRepository)
    {
      _webRepository = webRepository;
    }
    public abstract IDictionary<Outcome, IEnumerable<GenericOdd>> GetOdds(Uri matchURL, IDictionary<string, Outcome> teamOrPlayerToOutcome, DateTime timeStamp);
  }

  public class BestBettingOddsStrategy : AbstractOddsStrategy
  {
    public BestBettingOddsStrategy(IWebRepository webRepository)
      : base(webRepository)
    { }

    public override IDictionary<Outcome, IEnumerable<GenericOdd>> GetOdds(Uri matchURL, IDictionary<string, Outcome> teamOrPlayerToOutcome, DateTime timeStamp)
    {
      var outcomeDictionary = new Dictionary<Outcome, IEnumerable<GenericOdd>>();
      var oddsHTML = _webRepository.GetHTML(new Uri[] { matchURL }, s => Console.WriteLine(s)).First();
      var oddsTokens = WebUtils.ParseWebsite<BestBettingOddsCompetitor, BestBettingOdds>(
        oddsHTML, s => Console.WriteLine(s));

      var currentOutcome = Outcome.NotAssigned;
      var oddsForOutcome = new List<GenericOdd>();

      foreach (var oddsToken in oddsTokens)
      {
        if (oddsToken is BestBettingOddsCompetitor)
        {
          currentOutcome = teamOrPlayerToOutcome[((BestBettingOddsCompetitor)oddsToken).Competitor];

          oddsForOutcome = new List<GenericOdd>();
          outcomeDictionary.Add(currentOutcome, oddsForOutcome);
        }
        else
        {
          var odd = (BestBettingOdds)oddsToken;
          oddsForOutcome.Add(new BestBettingOdd()
          {
            DecimalOdds = odd.DecimalOdds,
            BookmakerName = odd.Bookmaker,
            Source = "BestBetting",
            TimeStamp = timeStamp,
            Priority = odd.Priority,
            ClickThroughURL = odd.ClickThroughURL
          });
        }
      }
      return outcomeDictionary;
    }
  }

  public class OddsCheckerMobiOddsStrategy : AbstractOddsStrategy
  {
    public OddsCheckerMobiOddsStrategy(IWebRepository webRepository)
      : base(webRepository)
    { }

    public override IDictionary<Outcome, IEnumerable<GenericOdd>> GetOdds(Uri matchURL, IDictionary<string, Outcome> teamOrPlayerToOutcome, DateTime timeStamp)
    {
      var outcomeDictionary = new Dictionary<Outcome, IEnumerable<GenericOdd>>();

      var html = _webRepository.GetHTML(new[] { matchURL }, s => Console.WriteLine(s), matchURL.ToString())
                               .First();

      var oddsTokens = WebUtils.ParseWebsite<OddsCheckerMobiCompetitor, OddsCheckerMobiOdds>(
        html, s => Console.WriteLine(s));

      var currentOutcome = Outcome.NotAssigned;
      var oddsForOutcome = new List<GenericOdd>();

      foreach (var oddsToken in oddsTokens)
      {
        if (oddsToken is OddsCheckerMobiCompetitor)
        {
          currentOutcome = teamOrPlayerToOutcome[((OddsCheckerMobiCompetitor)oddsToken).Outcome];

          oddsForOutcome = new List<GenericOdd>();
          outcomeDictionary.Add(currentOutcome, oddsForOutcome);
        }
        else
        {
          var odd = (OddsCheckerMobiOdds)oddsToken;
          oddsForOutcome.Add(new OddsCheckerOdd()
          {
            DecimalOdds = odd.DecimalOdds,
            BookmakerName = odd.Bookmaker,
            Source = "OddsChecker Mobi",
            TimeStamp = timeStamp,
            BetSlipValue = odd.BetSlipValue,
            Priority = odd.Priority
          });
        }
      }
      return outcomeDictionary;
    }
  }

  public class OddsCheckerWebOddsStrategy : AbstractOddsStrategy
  {
    public OddsCheckerWebOddsStrategy(IWebRepository webRepository)
      : base(webRepository)
    { }

    public override IDictionary<Outcome, IEnumerable<GenericOdd>> GetOdds(Uri matchURL, IDictionary<string, Outcome> teamOrPlayerToOutcome, DateTime timeStamp)
    {
      var outcomeDictionary = new Dictionary<Outcome, IEnumerable<GenericOdd>>();

      var html = _webRepository.GetHTML(new[] { matchURL }, s => Console.WriteLine(s), matchURL.ToString())
                               .First();

      var oddsTokens = WebUtils.ParseWebsite<OddsCheckerWebCompetitor, OddsCheckerWebOdds>(
        html, s => Console.WriteLine(s));

      var oddsCheckerJSFile = _webRepository.GetHTML(new[] { new Uri(@"http://static.oddschecker.com/javascript/betslip.js") }, s => Console.WriteLine(s))
                                      .First();
      var jint = new JintEngine();
      jint.Run(oddsCheckerJSFile);

      var currentOutcome = Outcome.NotAssigned;
      var oddsForOutcome = new List<GenericOdd>();

      foreach (var oddsToken in oddsTokens)
      {
        if (oddsToken is OddsCheckerWebCompetitor)
        {
          currentOutcome = teamOrPlayerToOutcome[((OddsCheckerWebCompetitor)oddsToken).Outcome];

          oddsForOutcome = new List<GenericOdd>();
          outcomeDictionary.Add(currentOutcome, oddsForOutcome);
        }
        else
        {
          var odd = (OddsCheckerWebOdds)oddsToken;

          var bSlip = jint.CallFunction("bSlip", odd.BookmakerID, odd.MarketIDOne, odd.MarketIDTwo, odd.OddsText).ToString();

          oddsForOutcome.Add(new OddsCheckerOdd()
          {
            DecimalOdds = odd.DecimalOdds,
            BookmakerName = odd.Bookmaker,
            Source = "OddsChecker Web",
            BetSlipValue = bSlip,
            TimeStamp = timeStamp,
            Priority = odd.Priority
          });
        }
      }
      return outcomeDictionary;
    }

  }

}
