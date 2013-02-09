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


namespace Samurai.Tests.DomainValue
{
  public class CouponStrategyTests
  {
    [TestFixture]
    public class GetTournaments
    {
      private List<AbstractCouponStrategy> tennisCouponStrategies;
      private List<AbstractCouponStrategy> footballCouponStrategies;
       
      [TestFixtureSetUp]
      public void SetUp()
      {
        //create all my coupon strategies
      }

      [Test, Category("CouponStrategyTests.GetTournaments")]
      public void CreatesACollectionOfFootballTournaments()
      {
        Assert.True(false);
      }

      [Test, Category("CouponStrategyTests.GetTournaments")]
      public void CreatesACollectionOfTennisTournaments()
      {
        Assert.True(false);
      }
    }

    [TestFixture]
    public class GetMatches
    {
      public List<AbstractCouponStrategy> couponStrategies;

      [TestFixtureSetUp]
      public void SetUp()
      {
        //create all my coupon strategies
      }

      [Test, Category("CouponStrategyTests.GetMatches")]
      public void CreatesACollectionOfFootballMatchesForTournament()
      {
        Assert.True(false);
      }

      [Test, Category("CouponStrategyTests.GetMatches")]
      public void ThrowsMissingFootballTeamAliasException()
      {
        Assert.True(false);
      }

      [Test, Category("CouponStrategyTests.GetMatches")]
      public void CreatesACollectionOfTennisMatchesForTournament()
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
