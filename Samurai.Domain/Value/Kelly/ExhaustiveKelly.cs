using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Value.Kelly
{
  public interface IKellyStrategy
  {
    IEnumerable<IBetable> CalculatedBets { get; }
    void CalculateKelly();
  }

  public class ExhaustiveKellyStrategy : IKellyStrategy
  {
    protected double kellyMultiplier;
    protected double minimumEdge;
    protected List<IBetable> calculatedBets;

    public IEnumerable<IBetable> CalculatedBets { get { return this.calculatedBets; } }

    public ExhaustiveKellyStrategy(IEnumerable<IBetable> potentialBets,
      double kellyMultiplier, double minimumEdge)
    {
      this.calculatedBets = potentialBets.ToList();
      this.kellyMultiplier = kellyMultiplier;
      this.minimumEdge = minimumEdge;
    }

    public virtual void CalculateKelly()
    {
      var singles = this.calculatedBets.Count();
      var bets = (int)Math.Pow(2, singles) - 1;
      double[] singleKellyStakes = new double[bets + 1];
      double[] realKellyStakes = new double[bets + 1];
      string[] parlayNames = new string[bets + 1];
      List<int>[] parlayMap = new List<int>[singles];
      var allBets = new Dictionary<string, double>();

      for (int i = 0; i <= singles - 1; i++)
      {
        var odds = this.calculatedBets[i].Odds;
        var prob = this.calculatedBets[i].Probability;
        var edge = this.calculatedBets[i].Edge;

        var win = odds - 1;

        if (edge >= this.minimumEdge)
        {
          singleKellyStakes[i] = Math.Max(
                                 (Math.Pow(win * prob, this.kellyMultiplier) -
                                  Math.Pow((1 - prob), this.kellyMultiplier)) /
                                 (Math.Pow(win * prob, this.kellyMultiplier) +
                                  win * Math.Pow(1 - prob, this.kellyMultiplier))
                                  , 0);
        }
        else
        {
          singleKellyStakes[i] = 0;
        }
        parlayMap[i] = new List<int>();
      }

      for (int i = 1; i <= bets; i++)
      {
        int parlaySize = ParlaySize(i);
        parlayMap[parlaySize - 1].Add(i);
      }

      for (int s = singles; s >= 1; s--)
      {
        var limit = parlayMap[s - 1].Count;
        for (int i = 0; i < limit; i++)
        {
          var parlayNumber = parlayMap[s - 1][i];
          realKellyStakes[parlayNumber] = 1;
          parlayNames[parlayNumber] = "";

          for (int k = 0; k <= singles - 1; k++)
          {
            if ((BitImp((int)Math.Pow(2, k), parlayNumber) == -1))
            {
              realKellyStakes[parlayNumber] *= singleKellyStakes[k];
              parlayNames[parlayNumber] += string.Format("{0}+", k + 1);
            }
          }
          for (int ss = s + 1; ss <= singles; ss++)
          {
            var ssLimit = parlayMap[ss - 1].Count;
            for (int ii = 0; ii < ssLimit; ii++)
            {
              var pp = parlayMap[ss - 1][ii];
              if ((BitImp(parlayNumber, pp) == -1))
                realKellyStakes[parlayNumber] -= realKellyStakes[pp];
            }
          }
          parlayNames[parlayNumber] = parlayNames[parlayNumber].Substring(0, parlayNames[parlayNumber].Length - 1);

          allBets.Add(parlayNames[parlayNumber], realKellyStakes[parlayNumber]);
        }
      }
      for (int i = 0; i < this.calculatedBets.Count; i++)
      {
        this.calculatedBets[i].AdjustedKellyStake = allBets[(i + 1).ToString()];
        this.calculatedBets[i].SingleKellyStake = singleKellyStakes[i];
      }

    }

    private int ParlaySize(int parlay)
    {
      var p = DecimalToBase(parlay, 2);

      var sizeRet = 0;
      for (int i = 0; i < p.Length; i++)
        sizeRet += int.Parse(p.Substring(i, 1));
      return sizeRet;
    }

    private string DecimalToBase(int dec, int numbase)
    {
      var bin = "";
      var result = new int[32];
      var maxBit = 32;
      char[] hexa = new char[] { 'A', 'B', 'C', 'D', 'E', 'F' };
      const int base10 = 10;


      for (; dec > 0; dec /= numbase)
      {
        int rem = dec % numbase;
        result[--maxBit] = rem;
      }
      for (int i = 0; i < result.Length; i++)
        if ((int)result.GetValue(i) >= base10)
          bin += hexa[(int)result.GetValue(i) % base10];
        else
          bin += result.GetValue(i);
      bin = bin.TrimStart(new char[] { '0' });
      return bin;
    }

    private int BitImp(int a, int b)
    {
      return (~a | b);
    }

  }
}
