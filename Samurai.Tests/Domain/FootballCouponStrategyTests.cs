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
  public class when_working_with_the_football_coupon_strategy : Specification
  {
    protected AbstractCouponStrategy couponStrategy;
    protected IWebRepository webRepository;
    protected M.Mock<IBookmakerRepository> bookmakerRepository;
    protected M.Mock<IFixtureRepository> fixtureRepository;
    protected M.Mock<IValueOptions> valueOptions;
    protected DateTime couponDate;
    protected IEnumerable<IGenericMatchCoupon> premCoupon;
    protected IEnumerable<IGenericMatchCoupon> champCoupon;
    protected IEnumerable<IGenericMatchCoupon> league1Coupon;
    protected IEnumerable<IGenericMatchCoupon> league2Coupon;

    protected override void Establish_context()
    {
      base.Establish_context();
      this.bookmakerRepository = new M.Mock<IBookmakerRepository>();
      this.fixtureRepository = new M.Mock<IFixtureRepository>();

      this.couponDate = new DateTime(2012, 10, 20);
      valueOptions = new M.Mock<IValueOptions>();
      this.valueOptions.Setup(t => t.CouponDate).Returns(this.couponDate);
      this.valueOptions.Setup(t => t.Sport).Returns(this.db.Sport["Football"]);

      this.bookmakerRepository.HasBasicMethods(this.db);
      this.fixtureRepository.HasBasicMethods(this.db);

      this.webRepository = new WebRepositoryTestData("Football/" + this.couponDate.ToShortDateString().Replace("/", "-"));
    }
  }

  public class and_using_the_oddschecker_web_coupon_strategy : when_working_with_the_football_coupon_strategy
  {
    protected override void Establish_context()
    {
      base.Establish_context();

      this.valueOptions.Setup(t => t.OddsSource).Returns(this.db.ExternalSource["Odds Checker Web"]);
      //need to override this, don't have data from 20th October..
      this.couponDate = new DateTime(2012, 11, 3);
      this.webRepository = new WebRepositoryTestData("Football/" + this.couponDate.ToShortDateString().Replace("/", "-"));
    }

    protected override void Because_of()
    {
      this.valueOptions.Setup(t => t.Tournament).Returns(this.db.Tournament["Premier League"]);
      this.couponStrategy = new OddsCheckerWebCouponStrategy<OddsCheckerWebCompetitionFootball>(this.bookmakerRepository.Object, this.fixtureRepository.Object, this.webRepository, this.valueOptions.Object);
      this.premCoupon = this.couponStrategy.GetMatches();
      
    }

    [Test]
    public void then_an_oddschecker_web_coupon_of_premier_league_football_matches_is_returned()
    {
      this.premCoupon.Count(m => m.MatchDate.Date == this.couponDate).ShouldEqual(7);
      //test selection
      var manUtdArsenal = this.premCoupon.FirstOrDefault(m => m.TeamOrPlayerA == "Man United" && m.TeamOrPlayerB == "Arsenal").HeadlineOdds;
      var sunderlandVilla = this.premCoupon.FirstOrDefault(m => m.TeamOrPlayerA == "Sunderland" && m.TeamOrPlayerB == "Aston Villa").HeadlineOdds;
      var westhamManCity = this.premCoupon.FirstOrDefault(m => m.TeamOrPlayerA == "West Ham" && m.TeamOrPlayerB == "Man City").HeadlineOdds;

      manUtdArsenal[Outcome.TeamOrPlayerA].ShouldApproximatelyEqual(1.0 + 4.0 / 6.0, 0.05);
      manUtdArsenal[Outcome.Draw].ShouldApproximatelyEqual(1.0 + 14.0 / 5.0, 0.05);
      manUtdArsenal[Outcome.TeamOrPlayerB].ShouldApproximatelyEqual(1.0 + 9.0 / 2.0, 0.05);

      sunderlandVilla[Outcome.TeamOrPlayerA].ShouldApproximatelyEqual(1.0 + 18.0 / 19.0, 0.05);
      sunderlandVilla[Outcome.Draw].ShouldApproximatelyEqual(1.0 + 13.0 / 5.0, 0.05);
      sunderlandVilla[Outcome.TeamOrPlayerB].ShouldApproximatelyEqual(1.0 + 3.0, 0.05);

      westhamManCity[Outcome.TeamOrPlayerA].ShouldApproximatelyEqual(1.0 + 9.0 / 2.0, 0.05);
      westhamManCity[Outcome.Draw].ShouldApproximatelyEqual(1.0 + 3.0, 0.05);
      westhamManCity[Outcome.TeamOrPlayerB].ShouldApproximatelyEqual(1.0 + 4.0 / 6.0, 0.05);


    }
  }

  public class and_using_the_oddschecker_mobi_coupon_strategy : when_working_with_the_football_coupon_strategy
  {
    protected override void Establish_context()
    {
      base.Establish_context();

      this.valueOptions.Setup(t => t.OddsSource).Returns(this.db.ExternalSource["Odds Checker Mobi"]);
    }

    protected override void Because_of()
    {
      this.valueOptions.Setup(t => t.Tournament).Returns(this.db.Tournament["Premier League"]);
      this.couponStrategy = new OddsCheckerMobiCouponStrategy<OddsCheckerMobiCompetitionFootball>(this.bookmakerRepository.Object, this.fixtureRepository.Object, this.webRepository, this.valueOptions.Object);
      this.premCoupon = this.couponStrategy.GetMatches();

      this.valueOptions.Setup(t => t.Tournament).Returns(this.db.Tournament["Championship"]);
      this.couponStrategy = new OddsCheckerMobiCouponStrategy<OddsCheckerMobiCompetitionFootball>(this.bookmakerRepository.Object, this.fixtureRepository.Object, this.webRepository, this.valueOptions.Object);
      this.champCoupon = this.couponStrategy.GetMatches();

      this.valueOptions.Setup(t => t.Tournament).Returns(this.db.Tournament["League One"]);
      this.couponStrategy = new OddsCheckerMobiCouponStrategy<OddsCheckerMobiCompetitionFootball>(this.bookmakerRepository.Object, this.fixtureRepository.Object, this.webRepository, this.valueOptions.Object);
      this.league1Coupon = this.couponStrategy.GetMatches();

      this.valueOptions.Setup(t => t.Tournament).Returns(this.db.Tournament["League Two"]);
      this.couponStrategy = new OddsCheckerMobiCouponStrategy<OddsCheckerMobiCompetitionFootball>(this.bookmakerRepository.Object, this.fixtureRepository.Object, this.webRepository, this.valueOptions.Object);
      this.league2Coupon = this.couponStrategy.GetMatches();

    }

    [Test]
    public void then_a_complete_coupon_of_premier_league_football_matches_is_returned()
    {
      var expectedMatches = new List<Tuple<string, string>>() 
      { 
        new Tuple<string, string>("Tottenham", "Chelsea"),
        new Tuple<string, string>("Fulham", "Aston Villa"),
        new Tuple<string, string>("Liverpool", "Reading"),
        new Tuple<string, string>("Man United", "Stoke"),
        new Tuple<string, string>("Swansea", "Wigan"),
        new Tuple<string, string>("West Brom", "Man City"),
        new Tuple<string, string>("West Ham", "Southampton"),
        new Tuple<string, string>("Norwich", "Arsenal")
      };
      foreach (var tuple in expectedMatches)
      {
        this.premCoupon.FirstOrDefault(m => m.TeamOrPlayerA == tuple.Item1 && m.TeamOrPlayerB == tuple.Item2).ShouldNotBeNull();
      }
    }
  }

  public class and_using_the_bestbetting_football_coupon_strategy : when_working_with_the_football_coupon_strategy
  {
    protected override void Establish_context()
    {
      base.Establish_context();

      this.valueOptions.Setup(t => t.OddsSource).Returns(this.db.ExternalSource["Best Betting"]);
    }

    protected override void Because_of()
    {
      this.valueOptions.Setup(t => t.Tournament).Returns(this.db.Tournament["Premier League"]);
      this.couponStrategy = new BestBettingCouponStrategy<BestBettingCompetitionFootball>(this.bookmakerRepository.Object, this.fixtureRepository.Object, this.webRepository, this.valueOptions.Object);
      this.premCoupon = this.couponStrategy.GetMatches();

      this.valueOptions.Setup(t => t.Tournament).Returns(this.db.Tournament["Championship"]);
      this.couponStrategy = new BestBettingCouponStrategy<BestBettingCompetitionFootball>(this.bookmakerRepository.Object, this.fixtureRepository.Object, this.webRepository, this.valueOptions.Object);
      this.champCoupon = this.couponStrategy.GetMatches();

      this.valueOptions.Setup(t => t.Tournament).Returns(this.db.Tournament["League One"]);
      this.couponStrategy = new BestBettingCouponStrategy<BestBettingCompetitionFootball>(this.bookmakerRepository.Object, this.fixtureRepository.Object, this.webRepository, this.valueOptions.Object);
      this.league1Coupon = this.couponStrategy.GetMatches();

      this.valueOptions.Setup(t => t.Tournament).Returns(this.db.Tournament["League Two"]);
      this.couponStrategy = new BestBettingCouponStrategy<BestBettingCompetitionFootball>(this.bookmakerRepository.Object, this.fixtureRepository.Object, this.webRepository, this.valueOptions.Object);
      this.league2Coupon = this.couponStrategy.GetMatches();

    }

    [Test]
    public void then_a_complete_coupon_of_premier_league_football_matches_is_returned()
    {
      this.premCoupon.Count(m => m.MatchDate.Date == this.couponDate).ShouldEqual(8);
      //test selection
      var spursChelseaOdds = this.premCoupon.FirstOrDefault(m => m.TeamOrPlayerA == "Tottenham" && m.TeamOrPlayerB == "Chelsea").HeadlineOdds;
      var manuStokeOdds = this.premCoupon.FirstOrDefault(m => m.TeamOrPlayerA == "Man United" && m.TeamOrPlayerB == "Stoke").HeadlineOdds;
      var norwichArsenalOdds = this.premCoupon.FirstOrDefault(m => m.TeamOrPlayerA == "Norwich" && m.TeamOrPlayerB == "Arsenal").HeadlineOdds;

      spursChelseaOdds[Outcome.TeamOrPlayerA].ShouldApproximatelyEqual(1.0 + 21.0 / 10.0, 0.05);
      spursChelseaOdds[Outcome.Draw].ShouldApproximatelyEqual(1.0 + 12.0 / 5.0, 0.05);
      spursChelseaOdds[Outcome.TeamOrPlayerB].ShouldApproximatelyEqual(1.0 + 7.0 / 4.0, 0.05);

      manuStokeOdds[Outcome.TeamOrPlayerA].ShouldApproximatelyEqual(1.0 + 1.0 / 3.0, 0.05);
      manuStokeOdds[Outcome.Draw].ShouldApproximatelyEqual(1.0 + 5.0, 0.05);
      manuStokeOdds[Outcome.TeamOrPlayerB].ShouldApproximatelyEqual(1.0 + 13.0, 0.05);

      norwichArsenalOdds[Outcome.TeamOrPlayerA].ShouldApproximatelyEqual(1.0 + 11.0 / 2.0, 0.05);
      norwichArsenalOdds[Outcome.Draw].ShouldApproximatelyEqual(1.0 + 7.0 / 2.0, 0.05);
      norwichArsenalOdds[Outcome.TeamOrPlayerB].ShouldApproximatelyEqual(1.0 + 3.0 / 5.0, 0.05);

    }

    [Test]
    public void then_a_complete_coupon_of_championship_football_matches_is_returned()
    {
      this.champCoupon.Count(m => m.MatchDate.Date == this.couponDate).ShouldEqual(11);
    }

    [Test]
    public void then_a_complete_coupon_of_league_1_football_matches_is_returned()
    {
      this.league1Coupon.Count(m => m.MatchDate.Date == this.couponDate).ShouldEqual(12);
    }

    [Test]
    public void then_a_complete_coupon_of_league_2_football_match_is_returned()
    {
      this.league2Coupon.Count(m => m.MatchDate.Date == this.couponDate).ShouldEqual(11);
    }
  }
}
