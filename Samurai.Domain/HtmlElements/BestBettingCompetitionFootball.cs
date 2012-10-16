using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;
using Samurai.Domain.Model;

namespace Samurai.Domain.HtmlElements
{
  public class BestBettingCompetitionFootball : IRegexableWebsite, IBestBettingCompetition
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
        regexs.Add(new Regex(@"\<a href=æ(?<PartURL>[^\?\s]+)\?showMostPopular=trueæ\>(?<CompetitionName>(Barclays Premier League|Npower Football League Championship|Npower Football League One|Npower Football League Two))\<"));
        return regexs;
      }
    }

    public bool Validates() { return true; }
    public void Clean()
    {
      CompetitionURL = new Uri("http://odds.bestbetting.com" + PartURL);
      if (CompetitionName == "Barclays Premier League")
        CompetitionType = "Premier League";
      else if (CompetitionName == "Npower Football League Championship")
        CompetitionType = "Championship";
      else if (CompetitionName == "Npower Football League One")
        CompetitionType = "League One";
      else if (CompetitionName == "Npower Football League Two")
        CompetitionType = "League Two";
    }

    public string ConvertTeamOrPlayerName(string teamOrPlayer)
    {
      return teamOrPlayer;
    }

    public override string ToString()
    {
      return CompetitionName;
    }
  }
}
