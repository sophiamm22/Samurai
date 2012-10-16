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
      var commision = 1.00;
      if (Bookmaker == "Betfair" || Bookmaker == "BETDAQ")
        commision = 0.95;
      else if (Bookmaker == "Smarkets")
        commision = 0.98;
      DecimalOdds =  1 + Math.Round(commision * Numerator / Denominator, 2);
      ClickThroughURL = new Uri(string.Format("http://odds.bestbetting.com/Clickthrough/?source={0}&id={1}&numerator={2}&denominator={3}&toolbars={4}&bookmakerId={5}",
        Source, ID, NumeratorString, DenominatorString, Toolbars, BookmakerID));

      var priorityLookup = new Dictionary<string, int>()
      {
         { "William Hill", 10000 },
         { "Stan James", 200 },
         { "Coral", 300000 },
         { "Ladbrokes", 400 },
         { "Paddy Power", 500 },
         { "Skybet", 600 },
         { "Sportingbet", 700 },
         { "totesport", 800 },
         { "Boylesports", 900 },
         { "BetVictor", 1000 },
         { "Blue Square", 1100 },
         { "Betfred", 1200 },
         { "Bet 365", 1300 },
         { "888 Sport", 1400 },
         { "Betinternet", 1500 },
         { "youwin", 1600 },
         { "Panbet", 1700 },
         { "10Bet", 1800 },
         { "32Red bet", 1900 },
         { "Bet770", 2000 },
         { "Bodog", 2100 },
         { "Pinnacle Sports", 2200 },
         { "Corbetts", 22500 },
         { "Betfair", 2300 },
         { "BETDAQ", 2400 },
         { "Smarkets", 11000 },
         { "Matchbook.com", 2600 },
         { "Spreadex", 1000000 }
      };
      Priority = priorityLookup[Bookmaker];
    }
  }
}
