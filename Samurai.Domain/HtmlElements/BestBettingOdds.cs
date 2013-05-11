using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;

namespace Samurai.Domain.HtmlElements
{
  public class BestBettingOdds : IRegexableWebsite
  {
    public string Bookmaker { get; set; }
    public string Source { get; set; }
    public string ID { get; set; }
    public string NumeratorString { get; set; }
    public string DenominatorString { get; set; }
    public string Toolbars { get; set; }
    public string BookmakerID { get; set; }

    public double Numerator { get; set; }
    public double Denominator { get; set; }
    public double DecimalOdds { get; set; }
    public Uri ClickThroughURL { get; set; }
    public int Priority { get; set; }

    public int Identifier { get; set; }

    public List<Regex> Regexs
    {
      get
      {
        return new List<Regex>()
        {
          new Regex(@"\<td title=æ(?<Bookmaker>[^æ]+)æ class=æ[^æ]+æ onclick=æa\((?<Source>[^,]+),(?<ID>[^,]+),(?<NumeratorString>[^,]+),(?<DenominatorString>[^,]+),(?<Toolbars>[^,]+),(?<BookmakerID>[^\(]+)\);")
        };
      }
    }

    public bool Validates() { return true; }
    public void Clean()
    {
      Numerator = double.Parse(NumeratorString);
      Denominator = double.Parse(DenominatorString);

      DecimalOdds = Math.Round(1 + Convert.ToDouble(Numerator) / Convert.ToDouble(Denominator), 2);

      ClickThroughURL = new Uri(string.Format("http://odds.bestbetting.com/Clickthrough/?source={0}&id={1}&numerator={2}&denominator={3}&toolbars={4}&bookmakerId={5}",
        Source, ID, NumeratorString, DenominatorString, Toolbars, BookmakerID));

    }
  }
}
