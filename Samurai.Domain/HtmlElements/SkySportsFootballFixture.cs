using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Samurai.Core;
using Samurai.Domain.Model;

namespace Samurai.Domain.HtmlElements
{
  public interface ISkySportsFixture
  {
    string KickOffTimeString { get; set; }
    string League { get; set; }
    string HomeTeam { get; set; }
    string AwayTeam { get; set; }
    string Score { get; set; }

    int HomeTeamScore { get; set; }
    int AwayTeamScore { get; set; }
    int KickOffHours { get; set; }
    int KickOffMintutes { get; set; }
    LeagueEnum LeagueEnum { get; set; }
  }

  public class SkySportsFootballFixture : IRegexableWebsite, ISkySportsFixture
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
          new Regex(@"\<div class=æscore-subæ\>(?<KickOffTimeString>[^\<]+)\</div\>[^\<]+\<div class=æscore-compæ\>[\s\n]+(?<League>(Barclays Prem|Sky Bet Ch'ship|Sky Bet League 1|Sky Bet League 2))[^\<]+\</div\>[^\<]+\<div class=æscore-side score-side1æ\>[\s\n]+(?<HomeTeam>[^\<]+)\</div\>[^\<]+\<div class=æscore-status æ\>vs\</div\>[^\<]+\<div class=æscore-side score-side2æ\>[\s\n]+(?<AwayTeam>[^\<]+)\</div\>")
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

      if (League == "Barclays Prem")
        LeagueEnum = Model.LeagueEnum.Premier;
      else if (League == "Sky Bet Ch'ship")
        LeagueEnum = Model.LeagueEnum.Championship;
      else if (League == "Sky Bet League 1")
        LeagueEnum = Model.LeagueEnum.League1;
      else
        LeagueEnum = Model.LeagueEnum.League2;

    }
  }
}
