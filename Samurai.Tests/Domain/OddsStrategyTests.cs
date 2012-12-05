using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M = Moq;
using NBehave.Spec.NUnit;
using NUnit.Framework;

using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Value;
using Samurai.Domain.Repository;
using Samurai.Domain.Entities;
using Samurai.Domain.Model;
using Samurai.Domain.HtmlElements;

namespace Samurai.Tests.Domain
{
  public class when_working_with_the_bestbetting_odds_strategy : and_using_the_bestbetting_football_coupon_strategy
  {
    protected AbstractOddsStrategy oddsStrategy;
    protected IDictionary<string, IDictionary<Outcome, IEnumerable<GenericOdd>>> returnedOdds;

    protected override void Establish_context()
    {
      base.Establish_context();
      this.returnedOdds = new Dictionary<string, IDictionary<Outcome, IEnumerable<GenericOdd>>>();
    }
  }

  public class and_using_the_bestbetting_odds_strategy : when_working_with_the_bestbetting_odds_strategy
  {
    protected override void Establish_context()
    {
      base.Establish_context();
      oddsStrategy = new BestBettingOddsStrategy(this.valueOptions.Object.Sport, this.bookmakerRepository.Object, this.fixtureRepository.Object, this.webRepository);
    }

    protected override void Because_of()
    {
      base.Because_of();
      foreach (var match in this.premCoupon.Where(m => m.MatchDate.Date == this.couponDate && m.TeamOrPlayerA.ToLower().IndexOf("swansea") < 0))//no idea why but swansea vs. wigan was missing
      {
        var matchIdentifier = string.Format("{0} vs. {1}", match.TeamOrPlayerA, match.TeamOrPlayerB);
        var odds = oddsStrategy.GetOdds(match, DateTime.Now);
        this.returnedOdds.Add(matchIdentifier, odds);
      }
    }
    [Test]
    public void then_an_bestbetting_odds_dictionary_of_premier_league_football_outcomes_is_returned()
    {
      throw new NotImplementedException();
    }
  }


  public class when_working_with_the_oddschecker_mobi_football_odds_strategy : and_using_the_oddschecker_mobi_coupon_strategy
  {
    protected AbstractOddsStrategy oddsStrategy;
    protected IDictionary<string, IDictionary<Outcome, IEnumerable<GenericOdd>>> returnedOdds;

    protected override void Establish_context()
    {
      base.Establish_context();
      this.returnedOdds = new Dictionary<string, IDictionary<Outcome, IEnumerable<GenericOdd>>>();
    }
  }

  public class and_using_the_oddschecker_mobi_odds_strategy : when_working_with_the_oddschecker_mobi_football_odds_strategy
  {
    protected override void Establish_context()
    {
      base.Establish_context();
      oddsStrategy = new OddsCheckerMobiOddsStrategy(this.valueOptions.Object.Sport, this.bookmakerRepository.Object, this.fixtureRepository.Object, this.webRepository);
    }

    protected override void Because_of()
    {
      base.Because_of();
      foreach (var match in this.premCoupon.Where(m => m.MatchDate.Date == this.couponDate))
      {
        var matchIdentifier = string.Format("{0} vs. {1}", match.TeamOrPlayerA, match.TeamOrPlayerB);
        var odds = oddsStrategy.GetOdds(match, DateTime.Now);
        this.returnedOdds.Add(matchIdentifier, odds);
      }
    }
    [Test]
    public void then_an_oddschecker_mobi_odds_dictionary_of_premier_league_football_outcomes_is_returned()
    {
      throw new NotImplementedException();
    }
  }

  public class when_working_with_the_oddschecker_web_football_odds_strategy : and_using_the_oddschecker_web_coupon_strategy
  {
    protected AbstractOddsStrategy oddsStrategy;
    protected IDictionary<string, IDictionary<Outcome, IEnumerable<GenericOdd>>> returnedOdds;

    protected override void Establish_context()
    {
      base.Establish_context();
      this.returnedOdds = new Dictionary<string, IDictionary<Outcome, IEnumerable<GenericOdd>>>();
    }
  }

  public class and_using_the_oddschecker_web_odds_strategy : when_working_with_the_oddschecker_web_football_odds_strategy
  {
    protected override void Establish_context()
    {
      base.Establish_context();
      oddsStrategy = new OddsCheckerWebOddsStrategy(this.valueOptions.Object.Sport, this.bookmakerRepository.Object, this.fixtureRepository.Object, this.webRepository);
    }

    protected override void Because_of()
    {
      base.Because_of();
      foreach (var match in this.premCoupon.Where(m => m.MatchDate.Date == this.couponDate))
      {
        var matchIdentifier = string.Format("{0} vs. {1}", match.TeamOrPlayerA, match.TeamOrPlayerB);
        var odds = oddsStrategy.GetOdds(match, DateTime.Now);
        this.returnedOdds.Add(matchIdentifier, odds);
      }
    }

    [Test]
    public void then_an_oddschecker_web_odds_dictionary_of_premier_league_football_outcomes_is_returned()
    {
      throw new NotImplementedException();
    }
  }
}
