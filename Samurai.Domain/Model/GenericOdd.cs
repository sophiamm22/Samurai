using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Domain.Entities;

namespace Samurai.Domain.Model
{
  public abstract class GenericOdd
  {
    public double DecimalOdds { get; set; }
    public string BookmakerName { get; set; }
    public string Source { get; set; }
    public Uri ClickThroughURL { get; set; }
    public DateTime TimeStamp { get; set; }
    public int Priority { get; set; }
    public void SetClickThroughURL(string uri)
    {
      ClickThroughURL = new Uri(uri);
    }
    public override string ToString()
    {
      return string.Format("{0:0.00} ({1}-{2})", DecimalOdds.ToString(), BookmakerName, Source.ToString());
    }
  }

  public class BestBettingOdd : GenericOdd
  {

  }

  public class OddsCheckerOdd : GenericOdd
  {
    public string BetSlipValue { get; set; }
  }
}
