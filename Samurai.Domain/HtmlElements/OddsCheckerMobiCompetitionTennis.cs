using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;
using Samurai.Domain.Model;

namespace Samurai.Domain.HtmlElements
{
  public class OddsCheckerMobiCompetitionTennis : IRegexableWebsite, IOddsCheckerCompetition
  {
    public string CompetitionName { get; set; }
    public string PartURL { get; set; }
    public Uri CompetitionURL { get; set; }
    public string CompetitionType { get; set; }

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
          new Regex(@"href=æ/tennis/mens-tour/atp-(?<PartURL>[^æ]+)æ\>(?<CompetitionName>(?!ATP World Tour Finals)[^\<]+)\<"),
          new Regex(@"href=æ/tennis/us-open/(?<PartURL>[^æ]+)æ\>(?<CompetitionName>(?!Womens US Open)[^\<]+)\<")
        };
      }
    }

    public bool Validates() { return true; }
    public void Clean()
    {
      if (CompetitionName == "Mens US Open")
        CompetitionURL = new Uri("http://oddschecker.mobi/tennis/us-open/mens-us-open");
      else
        CompetitionURL = new Uri(string.Format("http://oddschecker.mobi/tennis/mens-tour/atp-{0}",
          PartURL));
      CompetitionType = "ATP";
    }
  }


}
