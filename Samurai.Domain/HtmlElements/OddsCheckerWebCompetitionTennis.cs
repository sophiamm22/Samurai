using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Samurai.Core;
using Samurai.Domain.Model;

namespace Samurai.Domain.HtmlElements
{
  public class OddsCheckerWebCompetitionTennis : IRegexableWebsite, IOddsCheckerCompetition
  {
    public string CompetitionName { get; set; }
    public string PartURL { get; set; }
    public Uri CompetitionURL { get; set; }
    public String CompetitionType { get; set; }

    public string ConvertTeamOrPlayerName(string teamOrPlayer)
    {
      return teamOrPlayer;
    }

    public int Identifier { get; set; }

    public List<Regex> Regexs
    {
      get
      {
        return new List<Regex>()
        {
          new Regex(@"\<a title\=æ(?<CompetitionName>(?!ATP World Tour Finals)æ href=æ/tennis/mens-tour/atp-(?<PartURL>[^æ]+)æ\>ATP[^\<]+)\<")
        };
      }
    }

    public bool Validates() { return true; }
    public void Clean()
    {
      CompetitionURL = new Uri(string.Format("http://oddschecker.com/tennis/mens-tour/atp-{0}",
        PartURL));
      CompetitionType = "ATP";
    }

  }


}
