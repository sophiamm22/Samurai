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
  public class OddsCheckerWebScheduleMatch : IRegexableWebsite
  {
    public string PartURL { get; set; }
    public string TeamOrPlayerA { get; set; }
    public string TeamOrPlayerB { get; set; }

    public string TimeString { get; set; }

    public string BestOddsString { get; set; }
    public string GameState { get; set; }
    public string PayoutString { get; set; }

    public bool InPlay { get; set; }
    public Uri MatchURL { get; set; }
    public double Payout { get; set; }
    public IDictionary<Outcome, double> BestOdds { get; set; }

    public int Identifier { get; set; }
    public List<Regex> Regexs
    {
      get
      {
        return new List<Regex>()
        {
          new Regex(@"\<p[^\>]*\>(?<TimeString>\d{1,2}:\d{2})(\<img class=æ[^æ]+æ src=æ[^æ]+æ alt=æ[^æ]+æ width=æ[^æ]+æ height=æ[^æ]+æ\>|)\</p\>[^\<]+\</td\>[^\<]+(?<BestOddsString>(\<td data-participant-id=æ[^æ]+æ\>[^\<]+\<p\>\<span title=æ[^æ]+æ class=æ[^æ]+æ data-name=æ[^æ]+æ\>\</span\>\<span class=æfixtures-bet-nameæ\>[^\<]+\</span\>\<span class=æoddsæ\>[^\<]+\</span\>\</p\>[^\<]+\</td\>[^\<]+){2,3})\<td class=æbettingæ\>\<a title=æ[^æ]+æ class=æ[^æ]+æ href=æ(?<PartURL>[^æ]+)æ\>(?<GameState>(All Odds|In Play))\<span class=æ[^æ]+æ\>\</span\>")
          //deprecated - new Regex(@"\<td class=æcolTimeæ\>(?<TimeString>\d{1,2}:\d{2})\<br /\>(\<img src=æ[^\>]+\>|)\</td\>\<td class=ætdSpaceæ\> \</td\>(?<BestOddsString>(\<[^\<]+\>[^\<]+\<span class=æteamOddsæ\> \([^\)]+\)\</span\>\</td\>){2,3})\<td class=æcolImageæ\>\<a href=æ(?<PartURL>[^æ]+)æ\>")
        };
      }
    }

    public bool Validates() { return true; }
    public void Clean()
    {
      BestOdds = new Dictionary<Outcome, double>();
      //url
      MatchURL = new Uri("http://www.oddschecker.com" + PartURL);

      var oddsTokens = WebUtils.ParseWebsite<OddsCheckerWebScheduleMatchOdds>(BestOddsString, s => ProgressReporterProvider.Current.ReportProgress(s, ReporterImportance.Low, ReporterAudience.Admin))
                               .Cast<OddsCheckerWebScheduleMatchOdds>();

      if (oddsTokens.Count() == 3)
      {
        BestOdds.Add(Outcome.HomeWin, oddsTokens.ElementAt(0).Odds);
        BestOdds.Add(Outcome.Draw, oddsTokens.ElementAt(1).Odds);
        BestOdds.Add(Outcome.AwayWin, oddsTokens.ElementAt(2).Odds);
      }
      else
      {
        BestOdds.Add(Outcome.HomeWin, oddsTokens.ElementAt(0).Odds);
        BestOdds.Add(Outcome.AwayWin, oddsTokens.ElementAt(1).Odds);
      }
      TeamOrPlayerA = oddsTokens.First().TeamOrPlayer;
      TeamOrPlayerB = oddsTokens.Last().TeamOrPlayer;
      InPlay = GameState == "In Play";
    }
  }
}
