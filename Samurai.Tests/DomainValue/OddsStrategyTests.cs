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
  public class OddsStrategyTests
  {
    [TestFixture]
    public class GetOdds
    {
      private List<AbstractOddsStrategy> oddsStrategies;

      [TestFixtureSetUp]
      public void SetUp()
      {
        //create all my odds strategies
      }

      [Test, Category("OddsStrategyTests.GetOdds")]
      public void CreatesACollectionOfFootballOdds()
      {
      }
      [Test, Category("OddsStrategyTests.GetOdds")]
      public void CreatesACollectionOfTennisOdds()
      {
      }
    }
  }
}
