using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Value.Kelly
{
  public interface IKellyStrategyProvider
  {
    IKellyStrategy CreateKellyStrategy(IEnumerable<IBetable> potentialBets, double kellyMultiplier, double minimumEdge);
  }

  public class KellyStrategyProvider
  {
    public IKellyStrategy CreateKellyStrategy(IEnumerable<IBetable> potentialBets,
      double kellyMultiplier, double minimumEdge)
    {
      if (potentialBets.Count() <= 7)
        return new ExhaustiveKellyStrategy(potentialBets, kellyMultiplier, minimumEdge);
      else
        return new WhitrowKellyStrategy(potentialBets, kellyMultiplier, minimumEdge);
    }
  }
}
