using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;
using Samurai.Domain.Model;
using Samurai.Domain.Infrastructure;

namespace Samurai.Domain.HtmlElements
{
  public class BestBettingScheduleFinalURL : IRegexableWebsite
  {
    public string PartURL { get; set; }

    public Uri MatchURL { get; set; }

    public int Identifier { get; set; }
    public List<Regex> Regexs
    {
      get
      {
        var regexs = new List<Regex>();
        regexs.Add(new Regex(@"\<a href=æ(?<PartURL>[^æ]+)æ\>Match Result\</a\>"));
        return regexs;
      }
    }

    public bool Validates() { return true; }
    public void Clean()
    {
      MatchURL = new Uri("http://odds.bestbetting.com" + PartURL.Replace("-after-30-minutes", ""));

    }

  }
}
