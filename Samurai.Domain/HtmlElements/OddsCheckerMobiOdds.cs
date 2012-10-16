using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;

namespace Samurai.Domain.HtmlElements
{
  public class OddsCheckerMobiOdds : IRegexableWebsite
  {
    public string BetSlipValue { get; set; }
    public double DecimalOdds { get; set; }
    public string URLStub { get; set; }

    public string BookmakerCode { get; set; }
    public string MarketIDOne { get; set; }
    public string MarketIDTwo { get; set; }
    public string URLPart { get; set; }
    public string OddsText { get; set; }
    public string Bookmaker { get; set; }
    public int Priority { get; set; }

    public int Identifier { get; set; }

    public List<Regex> Regexs
    {
      get
      {
        return new List<Regex>()
        {
          new Regex(@"\<option value=æbetslip/(?<BookmakerCode>[^/]+)/(?<MarketIDOne>[^/]+)/(?<MarketIDTwo>[^/]+)/(?<URLPart>[^/]+)/(?<URLStub>[^æ]+)æ\>(?<OddsText>[^\s]+) (?<Bookmaker>[^\<]+)\</option\>")
        };
      }
    }

    public bool Validates() { return true; }
    public void Clean()
    {
      BetSlipValue = string.Format("betslip/{0}/{1}/{2}/{3}/{4}", BookmakerCode, MarketIDOne, MarketIDTwo, URLPart, URLStub);
      if (OddsText == "Evs") 
        DecimalOdds = 2.0;
      else
      {
        var numDen = OddsText.Split('/');
        DecimalOdds = numDen.Length == 1 ? Math.Round(1 + Convert.ToDouble(numDen[0])) : Math.Round(1 + Convert.ToDouble(numDen[0]) / Convert.ToDouble(numDen[1]), 2);
        if (Bookmaker == "WBX" || Bookmaker == "Betfair")
          DecimalOdds = (DecimalOdds - 1) * .95 + 1;
      }

      var priorityLookup = new Dictionary<string, int>()
      {
         { "William Hill", 10000 },
         { "Bet 365", 2 },
         { "Ladbrokes", 3 },
         { "Sky Bet", 4 },
         { "Paddy Power", 5 },
         { "Sporting Bet", 6 },
         { "Coral", 7000 },
         { "Stan James", 8 },
         { "Blue Square", 9 },
         { "Betfred", 10 },
         { "Totesport", 11 },
         { "888sport", 12 },
         { "Bet Victor", 13 },
         { "Bwin", 14 },
         { "Boylesports", 15 },
         { "Betfair", 16 },
         { "WBX", 17 }
      };
      Priority = priorityLookup[Bookmaker];
    }

  }
}
