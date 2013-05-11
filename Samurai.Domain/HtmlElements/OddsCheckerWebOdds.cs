using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;

namespace Samurai.Domain.HtmlElements
{
  public class OddsCheckerWebOdds : IRegexableWebsite
  {
    public double DecimalOdds { get; set; }
    public bool IsBestOdd { get; set; }

    public string OddsCheckerID { get; set; }
    public string BookmakerID { get; set; }
    public string OddsText { get; set; }
    public string OddsMetaData { get; set; }

    public int Identifier { get; set; }

    public List<Regex> Regexs
    {
      get
      {
        return new List<Regex>()
        {
          new Regex(@"\<td id=æ(?<OddsCheckerID>[^_]+)_(?<BookmakerID>[A-Z0-9]{2})æ class=æ(?<OddsMetaData>[^æ]+)æ\>(?<OddsText>[^\<]+)\</td\>")
          // deprecated - new Regex(@"\<td id=æ(?<OddsCheckerID>[^_]+)_(?<BookmakerID>[A-Z0-9]{2})æ class=æ[^æ]+æ onclick=æs\(this,'(?<MarketIDOne>[^']+)','(?<MarketIDTwo>[^']*)','[^']+'\);b\([^\(]+\);æ\>(?<OddsText>[^\<]+)\</td\>")
        };
      }
    }
    public bool Validates() { return true; }
    public void Clean()
    {
      if (OddsText == "Evs")
        DecimalOdds = 2.0;
      else
      {
        if (OddsText.IndexOf('-') >= 0)
          DecimalOdds = 1.01;
        else
        {
          var numDen = OddsText.Split('/');
          DecimalOdds = numDen.Length == 1 ? Math.Round(1 + Convert.ToDouble(numDen[0])) : Math.Round(1 + Convert.ToDouble(numDen[0]) / Convert.ToDouble(numDen[1]), 2);
        }
      }
      if (OddsMetaData.Length >= 3)
      {
        if (OddsMetaData.Substring(OddsMetaData.Length - 2) == " b")
          IsBestOdd = true;
      }
    }
    
  }
}
