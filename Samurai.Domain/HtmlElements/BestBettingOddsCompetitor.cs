using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;

namespace Samurai.Domain.HtmlElements
{
  public class BestBettingOddsCompetitor : IRegexableWebsite
  {
    public int Identifier { get; set; }

    public string CompetitorWhiteSpaced { get; set; }

    public string Competitor { get; set; }

    public List<Regex> Regexs
    {
      get
      {
        return new List<Regex>()
        {
          new Regex(@"\<td class=æCompetitoræ\>(?<CompetitorWhiteSpaced>[^\<]+)\</td\>")
        };
      }
    }

    public bool Validates() { return true; }
    public void Clean()
    {
      var competitorRaw = CompetitorWhiteSpaced.Replace("\t", "").Replace("\n", "").Trim();
      var c = competitorRaw.Split(',');
      Competitor = c.Length == 1 ? competitorRaw : (c[0] + ", " + c[1].Trim().Substring(0, 1)).RemoveDiacritics();
    }

  }
}
