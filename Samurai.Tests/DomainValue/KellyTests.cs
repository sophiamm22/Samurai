using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Samurai.Tests.TestInfrastructure;
using Samurai.Domain.Value.Kelly;

namespace Samurai.Tests.DomainValue
{
  public class KellyTests
  {
    [TestFixture]
    public class WhitrowKellyTester
    {
      private List<IBetable> bets;

      [TestFixtureSetUp]
      public void SetUp()
      {
        this.bets = new List<IBetable>()
        {
          new TestBetable(.65, 2.0),
          new TestBetable(.5, 3.0)
        };
      }

      [Test, Category("KellyTests.WhitrowKellyTester")]
      public void CreatesAWhitrowCalculator()
      {
        var provider = new KellyStrategyProvider();
        var calculator = provider.CreateKellyStrategy(bets, .25, 0);
        Assert.IsInstanceOf<WhitrowKellyStrategy>(calculator);
      }

      [Test, Category("KellyTests.WhitrowKellyTester")]
      public void CalculatesCorrectBetPercentages()
      {
        var calculator = new WhitrowKellyStrategy(this.bets, .25, 0);
        calculator.CalculateKelly();

      }
    }

    [TestFixture]
    public class ExhaustiveKellyTester
    {
      private List<IBetable> bets;

      [TestFixtureSetUp]
      public void SetUp()
      {
        this.bets = new List<IBetable>()
        {
          new TestBetable(.65, 2.0),
          new TestBetable(.5, 3.0),
        };
      }

      [Test, Category("KellyTests.ExhaustiveKellyTester")]
      public void CreatesAnExhaustiveKellyCalculator()
      {
        var provider = new KellyStrategyProvider();
        var calculator = provider.CreateKellyStrategy(bets, .25, 0);
        Assert.IsInstanceOf<ExhaustiveKellyStrategy>(calculator);
      }

      [Test, Category("KellyTests.ExhaustiveKellyTester")]
      public void CalculatesCorrectBetPercentages()
      {
        var calculator = new ExhaustiveKellyStrategy(this.bets, .25, 0);
        calculator.CalculateKelly();

      }

    }

  }
}
