using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;
using Samurai.Domain.Model;

namespace Samurai.Domain.HtmlElements
{
  public class BestBettingCompetitionTennis : IRegexableWebsite, IBestBettingCompetition
  {
    public string CompetitionName { get; set; }
    public string PartURL { get; set; }

    public Uri CompetitionURL { get; set; }
    public string CompetitionType { get; set; }

    public int Identifier { get; set; }
    public List<Regex> Regexs
    {
      get
      {
        var regexs = new List<Regex>();
        regexs.Add(new Regex(@"\<a href=æ(?<PartURL>[^\?\s]+)\?showMostPopular=trueæ\>(?<CompetitionName>[^\<]+)\<"));
        return regexs;
      }
    }

    public bool Validates() { return true; }
    public void Clean()
    {
      CompetitionURL = new Uri("http://odds.bestbetting.com" + PartURL);
      CompetitionType = "ATP";

    }

    public string ConvertTeamOrPlayerName(string teamOrPlayer)
    {
      var teamOrPlayerArray = teamOrPlayer.Split(',');
      return (teamOrPlayerArray[0] + ", " + teamOrPlayerArray[1].Trim().Substring(0, 1)).RemoveDiacritics();
    }

    public override string ToString()
    {
      return CompetitionName;
    }
  }
}
