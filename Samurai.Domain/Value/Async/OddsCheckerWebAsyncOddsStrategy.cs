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
using Samurai.Domain.Infrastructure;

namespace Samurai.Domain.Value.Async
{
  public class OddsCheckerWebAsyncOddsStrategy : AbstractAsyncOddsStrategy
  {
    public OddsCheckerWebAsyncOddsStrategy(Sport sport, IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepositoryProviderAsync webRepositoryProvider)
      : base(sport, bookmakerRepository, fixtureRepository, webRepositoryProvider)
    { }

    public override async Task<IDictionary<Outcome, IEnumerable<GenericOdd>>> GetOdds(GenericMatchCoupon matchCoupon, DateTime couponDate, DateTime timeStamp)
    {
      ProgressReporterProvider.Current.ReportProgress(string.Format("Getting Oddschecker Web odds for {0} vs. {1}", matchCoupon.TeamOrPlayerA, matchCoupon.TeamOrPlayerB), ReporterImportance.High, ReporterAudience.Admin);

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
      var missingBookmakerAlias = new List<MissingBookmakerAliasObject>();

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
          if (odd.BookmakerID == "SI" || odd.BookmakerID == "SX")
            continue;
          var bookmaker = this.bookmakerRepository.FindByOddsCheckerID(odd.BookmakerID);
          if (bookmaker == null)
          {
            missingBookmakerAlias.Add(new MissingBookmakerAliasObject
            {
              Bookmaker = odd.BookmakerID,

              ExternalSource = source.Source,
              ExternalSourceID = source.Id

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
