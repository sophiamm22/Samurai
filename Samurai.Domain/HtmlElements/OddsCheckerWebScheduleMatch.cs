using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;
using Samurai.Domain.Model;

namespace Samurai.Domain.HtmlElements
{
  public class OddsCheckerWebScheduleMatch : IRegexableWebsite
  {
    public string PartURL { get; set; }
    public string TeamOrPlayerA { get; set; }
    public string TeamOrPlayerB { get; set; }

    public string TimeString { get; set; }

    public string BestOddsString { get; set; }

    public string PayoutString { get; set; }

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
          new Regex(@"\<td class=æcolTimeæ\>(?<TimeString>\d{1,2}:\d{2})\<br /\>(\<img src=æ[^\>]+\>|)\</td\>\<td class=ætdSpaceæ\> \</td\>(?<BestOddsString>(\<[^\<]+\>[^\<]+\<span class=æteamOddsæ\> \([^\)]+\)\</span\>\</td\>){2,3})\<td class=æcolImageæ\>\<a href=æ(?<PartURL>[^æ]+)æ\>")
        };
      }
    }

    public bool Validates() { return true; }
    public void Clean()
    {
      BestOdds = new Dictionary<Outcome, double>();
      //url
      MatchURL = new Uri("http://http://www.oddschecker.com" + PartURL);

      var oddsTokens = WebUtils.ParseWebsite<OddsCheckerWebScheduleMatchOdds>(BestOddsString, s => Console.WriteLine(s))
                               .Cast<OddsCheckerWebScheduleMatchOdds>();

      if (oddsTokens.Count() == 3)
      {
        BestOdds.Add(Outcome.TeamOrPlayerA, oddsTokens.ElementAt(0).Odds);
        BestOdds.Add(Outcome.Draw, oddsTokens.ElementAt(1).Odds);
        BestOdds.Add(Outcome.TeamOrPlayerB, oddsTokens.ElementAt(2).Odds);
      }
      else
      {
        BestOdds.Add(Outcome.TeamOrPlayerA, oddsTokens.ElementAt(0).Odds);
        BestOdds.Add(Outcome.TeamOrPlayerB, oddsTokens.ElementAt(1).Odds);
      }
      TeamOrPlayerA = oddsTokens.First().TeamOrPlayer;
      TeamOrPlayerB = oddsTokens.Last().TeamOrPlayer;
    }
  }
}
