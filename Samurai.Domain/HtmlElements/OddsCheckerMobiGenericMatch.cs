using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;
using Samurai.Domain.Model;

namespace Samurai.Domain.HtmlElements
{
  public class OddsCheckerMobiGenericMatch : IRegexableWebsite
  {
    public string TeamOrPlayerA { get; set; }
    public string TeamOrPlayerB { get; set; }
    public string MatchPartURL { get; set; }
    public string GameState { get; set; }

    public Uri MatchURL { get; set; }
    public bool InPlay { get; set; }

    public int Identifier { get; set; }

    public List<Regex> Regexs
    {
      get
      {
        return new List<Regex>()
        {
          new Regex(@"<a class=ælist-view-link cell vertæ href=æ(?<MatchPartURL>[^æ]+)æ title=æ[^æ]+æ data-date-time=æ[^æ]+æ\>\d{1,2}:\d{1,2} (?<TeamOrPlayerA>[^/]+) v (?<TeamOrPlayerB>[^\</]+)\</a\>")
        };
      }
    }

    public bool Validates() { return true; }
    public void Clean()
    {
      MatchURL = new Uri(@"http://m.oddschecker.com" + MatchPartURL);
      TeamOrPlayerA = TeamOrPlayerA.Replace("&amp;", "&");
      TeamOrPlayerB = TeamOrPlayerB.Replace("&amp;", "&");
      InPlay = !string.IsNullOrEmpty(GameState);
    }

  }
}
