using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;

namespace Samurai.Domain.HtmlElements
{
  public class OddsCheckerWebMarketID : IRegexableWebsite
  {
    public string MarketID { get; set; }

    public int Identifier { get; set; }

    public List<Regex> Regexs
    {
      get
      {
        return new List<Regex>()
        {
          new Regex(@"\<table data-market-id=æ(?<MarketID>[^æ]+)æ class=æeventTable æ\>")
        };
      }
    }
    public bool Validates() { return true; }
    public void Clean()
    {
      return;
    }

  }
}
