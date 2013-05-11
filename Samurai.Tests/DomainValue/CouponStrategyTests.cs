using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;

using Samurai.Domain.Value;
using E = Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Domain.APIModel;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Repository;
using Samurai.Tests.TestInfrastructure;
using Samurai.Tests.TestInfrastructure.MockBuilders;
using Samurai.Domain.HtmlElements;
using Samurai.Domain.Model;


namespace Samurai.Tests.DomainValue
{
  public class CouponStrategyTests
  {
    [TestFixture]
    public class CouponTester
    {
      protected Dictionary<string, IEnumerable<GenericMatchCoupon>> tournamentsToTest;

      protected AbstractCouponStrategy bestBettingFootballStrategy;
      protected AbstractCouponStrategy bestBettingTennisStrategy;
      protected AbstractCouponStrategy oddsCheckerMobiFootballStrategy;
      protected AbstractCouponStrategy oddsCheckerMobiTennisStrategy;
      protected AbstractCouponStrategy oddsCheckerWebFootballStrategy;
      protected AbstractCouponStrategy oddsCheckerWebTennisStrategy;

      protected Mock<IBookmakerRepository> mockBookmakerRepository;
      protected Mock<IFixtureRepository> mockFixtureRepository;
      protected IWebRepositoryProvider webRepositoryProvider;
      protected IValueOptions valueOptions;

      protected List<string> tennisTournaments;
      protected List<string> oddsSources;

      protected Dictionary<string, Uri> tournamentCouponUriLookup;
      protected Dictionary<string, AbstractCouponStrategy> couponStrategies;
      
      [TestFixtureSetUp]
      public void SetUp()
      {
        this.tournamentsToTest = new Dictionary<string, IEnumerable<GenericMatchCoupon>>();

        this.tournamentCouponUriLookup = new Dictionary<string, Uri>();
        this.tournamentCouponUriLookup.Add("VTR Open|Best Betting", new Uri("http://odds.bestbetting.com/tennis/atp-vtr-open/"));
        this.tournamentCouponUriLookup.Add("VTR Open|Odds Checker Web", new Uri("http://www.oddschecker.com/tennis/atp-vina-del-mar"));
        this.tournamentCouponUriLookup.Add("VTR Open|Odds Checker Mobi", new Uri("http://oddschecker.mobi/tennis/mens-tour/atp-vina-del-mar"));
        this.tournamentCouponUriLookup.Add("PBZ Zagreb Indoors|Best Betting", new Uri("http://odds.bestbetting.com/tennis/pbz-zagreb-indoors/"));
        this.tournamentCouponUriLookup.Add("PBZ Zagreb Indoors|Odds Checker Web", new Uri("http://www.oddschecker.com/tennis/atp-zagreb"));
        this.tournamentCouponUriLookup.Add("PBZ Zagreb Indoors|Odds Checker Mobi", new Uri("http://oddschecker.mobi/tennis/mens-tour/atp-zagreb"));
        this.tournamentCouponUriLookup.Add("Open Sud de France|Best Betting", new Uri("http://odds.bestbetting.com/tennis/open-sud-de-france/"));
        this.tournamentCouponUriLookup.Add("Open Sud de France|Odds Checker Web", new Uri("http://www.oddschecker.com/tennis/atp-montpellier"));
        this.tournamentCouponUriLookup.Add("Open Sud de France|Odds Checker Mobi", new Uri("http://oddschecker.mobi/tennis/mens-tour/atp-montpellier"));

        this.tennisTournaments = new List<string>() { "VTR Open", "PBZ Zagreb Indoors", "Open Sud de France" };
        this.oddsSources = new List<string>() { "Best Betting", "Odds Checker Web", "Odds Checker Mobi" };

        this.webRepositoryProvider = new ManifestWebRepositoryProvider();

        this.mockFixtureRepository = BuildFixtureRepository.Create()
          .CanGetExternalSource()
          .HasGetAliasWhichReturnsItself();

        this.mockBookmakerRepository = BuildBookmakerRepository.Create()
          .ReturnsTournamentCouponURLs(this.tournamentCouponUriLookup);

        this.valueOptions = new ValueOptions();

        this.bestBettingFootballStrategy = new BestBettingCouponStrategy<BestBettingCompetitionFootball>(this.mockBookmakerRepository.Object, this.mockFixtureRepository.Object, this.webRepositoryProvider, this.valueOptions);
        this.bestBettingTennisStrategy = new BestBettingCouponStrategy<BestBettingCompetitionTennis>(this.mockBookmakerRepository.Object, this.mockFixtureRepository.Object, this.webRepositoryProvider, this.valueOptions);

        this.oddsCheckerMobiFootballStrategy = new OddsCheckerMobiCouponStrategy<OddsCheckerMobiCompetitionFootball>(this.mockBookmakerRepository.Object, this.mockFixtureRepository.Object, this.webRepositoryProvider, this.valueOptions);
        this.oddsCheckerMobiTennisStrategy = new OddsCheckerMobiCouponStrategy<OddsCheckerMobiCompetitionTennis>(this.mockBookmakerRepository.Object, this.mockFixtureRepository.Object, this.webRepositoryProvider, this.valueOptions);

        this.oddsCheckerWebFootballStrategy = new OddsCheckerWebCouponStrategy<OddsCheckerWebCompetitionFootball>(this.mockBookmakerRepository.Object, this.mockFixtureRepository.Object, this.webRepositoryProvider, this.valueOptions);
        this.oddsCheckerWebTennisStrategy = new OddsCheckerWebCouponStrategy<OddsCheckerWebCompetitionTennis>(this.mockBookmakerRepository.Object, this.mockFixtureRepository.Object, this.webRepositoryProvider, this.valueOptions);

        this.couponStrategies = new Dictionary<string, AbstractCouponStrategy>()
        {
          { "Best Betting|Football", this.bestBettingFootballStrategy },
          { "Best Betting|Tennis", this.bestBettingTennisStrategy },
          { "Odds Checker Mobi|Football", this.oddsCheckerMobiFootballStrategy },
          { "Odds Checker Mobi|Tennis", this.oddsCheckerMobiTennisStrategy },
          { "Odds Checker Web|Football", this.oddsCheckerWebFootballStrategy },
          { "Odds Checker Web|Tennis", this.oddsCheckerWebTennisStrategy }
        };

      }

      protected void UpdateValueOptions(string sport, string tournament,
        string oddsSource, DateTime fixtureDate)
      {
        this.valueOptions.CouponDate = fixtureDate;
        this.valueOptions.OddsSource = new E.ExternalSource() { Source = oddsSource };
        this.valueOptions.Sport = new E.Sport() { SportName = sport };
        this.valueOptions.Tournament = new E.Tournament() { TournamentName = tournament };
      }
    }


    public class GetTournaments : CouponTester
    {

      [Test, Category("CouponStrategyTests.GetTournaments")]
      public void CreatesACollectionOfBestBettingFootballTournaments()
      {
        UpdateValueOptions("Football", "Doesnt matter", "Best Betting", new DateTime(2013, 02, 06));
        var tournaments = this.bestBettingFootballStrategy.GetTournaments();
      }

      [Test, Category("CouponStrategyTests.GetTournaments")]
      public void CreatesACollectionOfTennisTournaments()
      {
        Assert.True(false);
      }
    }

    [TestFixture]
    public class GetMatches : CouponTester
    {
      [Test, Category("CouponStrategyTests.GetMatches")]
      public void CanGetAllTennisTournamentCouponsFromStrategies()
      {
        foreach (var oddsSource in this.oddsSources)
        {
          foreach (var tournament in this.tennisTournaments)
          {
            UpdateValueOptions("Tennis", tournament, oddsSource, new DateTime(2013, 02, 06));
            var couponStrategy = this.couponStrategies[string.Format("{0}|{1}", oddsSource, "Tennis")];
            var theseMatches = couponStrategy.GetMatches();
            this.tournamentsToTest.Add(string.Format("{0}|{1}", tournament, oddsSource), theseMatches);
            Assert.IsTrue(theseMatches.Count() > 0); //simple test to say that we at least have something
          }
        }

        TestBestBettingVTROpen();
        TestBestBettingZagrebIndoors();
        TestBestBettingOpenSud();

        TestOddsCheckerWebVTROpen();
        TestOddsCheckerWebZagrebIndoors();
        TestOddsCheckerWebOpenSud();

        TestOddsCheckerMobiVTROpen();
        TestOddsCheckerMobiZagrebIndoors();
        TestOddsCheckerMobiOpenSud();

      }

      private void TestBestBettingVTROpen()
      {
      }
      private void TestBestBettingZagrebIndoors()
      {
      }
      private void TestBestBettingOpenSud()
      {
      }
      private void TestOddsCheckerWebVTROpen()
      {
      }
      private void TestOddsCheckerWebZagrebIndoors()
      {
      }
      private void TestOddsCheckerWebOpenSud()
      {
      }
      private void TestOddsCheckerMobiVTROpen()
      {
      }
      private void TestOddsCheckerMobiZagrebIndoors()
      {
      }
      private void TestOddsCheckerMobiOpenSud()
      {
      }

      [Test, Category("CouponStrategyTests.GetMatches")]
      public void ThrowsMissingFootballTeamAliasException()
      {
        Assert.True(false);
      }

      [Test, Category("CouponStrategyTests.GetMatches")]
      public void CanGetAllFootballTournamentCouponsFromStrategies()
      {
        Assert.True(false);
      }

      [Test, Category("CouponStrategyTests.GetMatches")]
      public void ThrowsMissingTennisPlayerAliasException()
      {
        Assert.True(false);
      }


    }
  }
}
