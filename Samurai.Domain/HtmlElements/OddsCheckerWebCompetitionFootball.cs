using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;
using Samurai.Domain.Model;

namespace Samurai.Domain.HtmlElements
{
  public class OddsCheckerWebCompetitionFootball : IRegexableWebsite, IOddsCheckerCompetition
  {
    public string CompetitionName { get; set; }
    public string PartURL { get; set; }
    public Uri CompetitionURL { get; set; }
    public String CompetitionType { get; set; }

    public int Identifier { get; set; }

    public string ConvertTeamOrPlayerName(string teamOrPlayer)
    {
      return teamOrPlayer;
    }

    public List<Regex> Regexs
    {
      get
      {
        return new List<Regex>()
        {
          new Regex(@"\<a title=æ(?<CompetitionName>[^æ]+)æ href=æ/football/english/(?<PartURL>(premier-league|championship|league-1|league-2)æ\>[^\<]\<")
        };
      }
    }

    public bool Validates() { return true; }
    public void Clean()
    {
      CompetitionURL = new Uri(string.Format("http://oddschecker.com/football/english/{0}",
        PartURL));
      if (PartURL == "premier-league")
        CompetitionType = "Premier League";
      else if (PartURL == "championship")
        CompetitionType = "Championship";
      else if (PartURL == "league-1")
        CompetitionType = "League One";
      else if (PartURL == "league-2")
        CompetitionType = "League Two";
    }
  }


}
