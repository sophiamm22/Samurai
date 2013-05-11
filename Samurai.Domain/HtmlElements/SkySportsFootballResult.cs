using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;
using Samurai.Domain.Model;

namespace Samurai.Domain.HtmlElements
{
  public class SkySportsFootballResult : IRegexableWebsite, ISkySportsFixture
  {
    public int Identifier { get; set; }

    public string KickOffTimeString { get; set; }
    public string League { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
    public string Score { get; set; }

    public int HomeTeamScore { get; set; }
    public int AwayTeamScore { get; set; }
    public int KickOffHours { get; set; }
    public int KickOffMintutes { get; set; }
    public LeagueEnum LeagueEnum { get; set; }

    public List<Regex> Regexs
    {
      get
      {
        return new List<Regex>()
        {
          new Regex(@"\<div class=æscore-subæ\>(?<KickOffTimeString>[^\<]+)\</div\>[^\<]+\<div class=æscore-compæ\>[\s\n]+(?<League>(Barclays Prem|Championship|League 1|League 2))[^\<]+\</div\>[^\<]+\<div class=æscore-side score-side1æ\>[\s\n]+(?<HomeTeam>[^\<]+)\<ul class=æscorersæ\>[^<]+(\<li\>[^\<]+\</li\>[^\<]+){1,}\</ul\>[^\<]+\</div\>[^\<]+\<div class=æscore-status score-postæ\>(?<Score>[^\<]+)\</div\>[^\<]+\<div class=æscore-side score-side2æ\>[\s\n]+(?<AwayTeam>[^\<]+)"),
          new Regex(@"\<div class=æscore-subæ\>(?<KickOffTimeString>[^\<]+)\</div\>[^\<]+\<div class=æscore-compæ\>[\s\n]+(?<League>(Barclays Prem|Championship|League 1|League 2))[^\<]+\</div\>[^\<]+\<div class=æscore-side score-side1æ\>[\s\n]+(?<HomeTeam>[^\<]+)\</div\>[^\<]+\<div class=æscore-status score-postæ\>(?<Score>[^\<]+)\</div\>[^\<]+\<div class=æscore-side score-side2æ\>[\s\n]+(?<AwayTeam>[^\<]+)\<")
        };
      }
    }
    public bool Validates() { return true; }
    public void Clean()
    {
      HomeTeam = HomeTeam.Trim();
      AwayTeam = AwayTeam.Trim();

      var timeSplit = KickOffTimeString.Split(':');
      KickOffHours = int.Parse(timeSplit[0]);
      KickOffMintutes = int.Parse(timeSplit[1]);

      var scoreSplit = Score.Replace(" - ", "-").Split('-');
      HomeTeamScore = int.Parse(scoreSplit[0]);
      AwayTeamScore = int.Parse(scoreSplit[1]);

      if (League == "Barclays Prem")
        LeagueEnum = Model.LeagueEnum.Premier;
      else if (League == "Championship")
        LeagueEnum = Model.LeagueEnum.Championship;
      else if (League == "League 1")
        LeagueEnum = Model.LeagueEnum.League1;
      else
        LeagueEnum = Model.LeagueEnum.League2;
    }

  }
}
