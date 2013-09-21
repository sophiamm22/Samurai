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
  public class OddsCheckerMobiAsyncOddsStrategy : AbstractAsyncOddsStrategy
  {
    public OddsCheckerMobiAsyncOddsStrategy(Sport sport, IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepositoryProviderAsync webRepositoryProvider)
      : base(sport, bookmakerRepository, fixtureRepository, webRepositoryProvider)
    { }

    public override async Task<IDictionary<Outcome, IEnumerable<GenericOdd>>> GetOdds(GenericMatchCoupon matchCoupon, DateTime couponDate, DateTime timeStamp)
    {
      ProgressReporterProvider.Current.ReportProgress(string.Format("Getting Oddschecker Mobile odds for {0} vs. {1}", matchCoupon.TeamOrPlayerA, matchCoupon.TeamOrPlayerB), ReporterImportance.High, ReporterAudience.Admin);

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
      var missingBookmakerAlias = new List<MissingBookmakerAliasObject>();

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
            missingBookmakerAlias.Add(new MissingBookmakerAliasObject
            {
              Bookmaker = odd.Bookmaker,

              ExternalSource = source.Source,
              ExternalSourceID = source.Id
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
}
