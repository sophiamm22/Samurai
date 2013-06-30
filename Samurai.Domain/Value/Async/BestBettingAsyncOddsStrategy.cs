﻿using System;
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
  public class BestBettingAsyncOddsStrategy : AbstractAsyncOddsStrategy
  {
    public BestBettingAsyncOddsStrategy(Sport sport, IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepositoryProviderAsync webRepositoryProvider)
      : base(sport, bookmakerRepository, fixtureRepository, webRepositoryProvider)
    { }

    public override async Task<IDictionary<Outcome, IEnumerable<GenericOdd>>> GetOdds(GenericMatchCoupon matchCoupon, DateTime couponDate, DateTime timeStamp)
    {
      ProgressReporterProvider.Current.ReportProgress(string.Format("Getting Best Betting odds for {0} vs. {1}", matchCoupon.TeamOrPlayerA, matchCoupon.TeamOrPlayerB), ReporterImportance.High, ReporterAudience.Admin);
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

}
