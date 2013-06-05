using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Value.Kelly;

namespace Samurai.Tests.TestInfrastructure
{
  public class TestBetable : IBetable
  {
    private double probability;
    public double Probability { get { return this.probability; } }

    private double odds;
    public double Odds { get { return this.odds; } }
    
    public double BookmakerImpliedProbability 
    {
      get { return 1.0 / this.odds; }
    }

    public double Edge 
    {
      get { return this.probability * this.odds - 1; } 
    }

    public double SingleKellyStake { get; set; }
    public double AdjustedKellyStake { get; set; }

    public TestBetable(double probability, double odds)
    {
      this.probability = probability;
      this.odds = odds;
    }
  }
}
