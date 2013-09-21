using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Core;

namespace Samurai.Domain.Value.Kelly
{
  public class WhitrowKellyStrategy : ExhaustiveKellyStrategy
  {
    public WhitrowKellyStrategy(IEnumerable<IBetable> potentialBets,
      double kellyMultiplier, double minimumEdge)
      : base(potentialBets, kellyMultiplier, minimumEdge)
    {
    }

    public override void CalculateKelly()
    {
      CalcSingleKelly();
      NormaliseSingleKelly();

      var trials = 200;
      var runs = 200;
      var learningSteps = 300;
      var noBets = this.calculatedBets.Count;

      for (int run = 0; run < runs; run++)
      {
        double[] lastRate = new double[noBets];
        double[] rate = new double[noBets];
        double[] step = new double[noBets];
        double[] update = new double[noBets];
        double[] proposed = new double[noBets];
        double[] lastSlope = new double[noBets];
        var slope = Slopes(trials, noBets);

        for (int b = 0; b < noBets; b++)
        {
          if (run == 0)
            rate[b] = 1;
          else
            rate[b] = LearningRate(lastSlope[b], slope[b], lastSlope[b]);

          var singleKellyStake = this.calculatedBets[b].SingleKellyStake;
          var adjustedKellyStake = this.calculatedBets[b].AdjustedKellyStake;

          step[b] = (singleKellyStake - adjustedKellyStake) / (double)learningSteps;
          update[b] = Math.Sign(slope[b]) * rate[b] * step[b];
          proposed[b] = adjustedKellyStake + update[b];
        }
        var totalProposedStake = proposed.Sum();
        var rescaleFactor = 0.0;
        if (totalProposedStake > 1)
          rescaleFactor = 0.99 / totalProposedStake;
        else
          rescaleFactor = 1;

        for (int b = 0; b < noBets; b++)
          this.calculatedBets[b].AdjustedKellyStake = proposed[b] < 0 ? 0 : (proposed[b] * rescaleFactor);

        lastRate = rate;
      }
      var s = this.calculatedBets.Sum(b => b.AdjustedKellyStake) * this.kellyMultiplier;

      foreach (var bet in this.calculatedBets)
      {
        bet.AdjustedKellyStake = this.kellyMultiplier * bet.AdjustedKellyStake;
      }
    }

    private void CalcSingleKelly()
    {
      foreach (var bet in this.calculatedBets)
      {
        var win = bet.Odds - 1;
        var prob = bet.Probability;
        if (bet.Edge >= this.minimumEdge)
        {
          bet.SingleKellyStake = Math.Max(
                                          ((win * prob) -
                                           (1 - prob)) /
                                          ((win * prob) +
                                           win * (1 - prob))
                                           , 0);
        }
        else
        {
          bet.SingleKellyStake = 0;
        }
      }
    }

    private void NormaliseSingleKelly()
    {
      var sumKelly = this.calculatedBets.Sum(b => b.SingleKellyStake);
      if (sumKelly > 1)
      {
        foreach (var bet in this.calculatedBets)
          bet.AdjustedKellyStake = bet.SingleKellyStake / sumKelly;
      }
      else
      {
        foreach (var bet in this.calculatedBets)
          bet.AdjustedKellyStake = bet.SingleKellyStake;
      }

    }

    private double[] Slopes(int trials, int noBets)
    {
      var rnd = new Random(1);
      double[] jointPlusMinus = new double[trials];
      double[] rs = new double[trials];
      double[] growthContribution = new double[trials];
      double[,] zOverR = new double[trials, noBets];
      double[] slope = new double[noBets];

      for (int trial = 0; trial < trials; trial++)
      {
        double[] z = new double[noBets];
        double[] zx = new double[noBets];

        for (int b = 0; b < noBets; b++)
        {
          var bet = this.calculatedBets[b];
          var r = rnd.NextDouble();
          if (r < bet.Probability)
          {
            z[b] = bet.Odds - 1;
            zx[b] = (bet.Odds - 1) * bet.AdjustedKellyStake;
          }
          else
          {
            z[b] = -1.0;
            zx[b] = -bet.AdjustedKellyStake;
          }
        }
        jointPlusMinus[trial] = zx.Sum();
        rs[trial] = jointPlusMinus[trial] + 1;
        growthContribution[trial] = Math.Log(rs[trial]) / (double)trials;
        for (int b = 0; b < noBets; b++)
        {
          zOverR[trial, b] = z[b] / ((double)trials * rs[trial]);
        }
      }
      for (int b = 0; b <= noBets - 1; b++)
      {
        slope[b] = zOverR.ColumnSum(b);
      }
      return slope;
    }

    private double LearningRate(double slopePrevious, double slope, double ratePrevious)
    {
      return Math.Sign(slopePrevious) == Math.Sign(slope) ? 1.05 * ratePrevious : 0.95 * ratePrevious;
    }

  }
}
