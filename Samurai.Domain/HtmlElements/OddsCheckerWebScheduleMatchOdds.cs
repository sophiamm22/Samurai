using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;
using Samurai.Domain.Model;

namespace Samurai.Domain.HtmlElements
{
  public class OddsCheckerWebScheduleMatchOdds : IRegexableWebsite
  {
    public string TeamOrPlayer { get; set; }
    public string OddsString { get; set; }

    public double Odds { get; set; }

    public int Identifier { get; set; }
    public List<Regex> Regexs
    {
      get
      {
        return new List<Regex>()
        {
          new Regex(@"\<td class=[^\<]+\>(?<TeamOrPlayer>[^\<]+)\<span class=[^\<]+\> \((?<OddsString>[0-9/]+)\)\</span\>\</td\>")
        };
      }
    }
    public bool Validates() { return true; }
    public void Clean()
    {
      if (OddsString.IndexOf('/') > 0)
      {
        var oddsSplit = OddsString.Split('/');
        double outNum = 0;
        double outDen = 0;
        if (double.TryParse(oddsSplit[0], out outNum) && double.TryParse(oddsSplit[1], out outDen))
          Odds = 1 + outNum / outDen;
      }
      else
      {
        double outSingle = 0;
        if (double.TryParse(OddsString, out outSingle))
          Odds = 1 + outSingle;
      }
    }
  }
}
