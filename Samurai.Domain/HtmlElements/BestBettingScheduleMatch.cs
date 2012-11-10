using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;
using Samurai.Domain.Model;

namespace Samurai.Domain.HtmlElements
{
  public class BestBettingScheduleMatch : IRegexableWebsite
  {
    public string PartURL { get; set; }
    public string TeamOrPlayerA { get; set; }
    public string TeamOrPlayerB { get; set; }

    public string TimeString { get; set; }
    public string TimeZone { get; set; }

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
        var regexs = new List<Regex>();
        regexs.Add(new Regex(@"\<td class=æfirstColumnæ\>[^\<]+\<a href=æ(?<PartURL>[^æ]+)æ>(?<TeamOrPlayerA>[A-Za-z\.\-, &]+) v (?<TeamOrPlayerB>[A-Za-z\.\-, &]+)\</a\>[^\(]+\((?<TimeString>\d{2}:\d{2})&nbsp;(?<TimeZone>[A-Z]{3})\)[^\<]+\</td\>[^\<]+(?<BestOddsString>(\<td id=æ[^æ]+æ class=æselectionBestOddæ style=æ æ\>[^0-9\.]+[0-9\.]+[^\<]+\</td\>[^\<]+){2,3})\<td id=æ[^æ]+æ class=æbppWidth bppæ\>(?<PayoutString>\d+\.\d+) %\</td\>"));
        return regexs;
      }
    }

    public bool Validates() { return true; }
    public void Clean()
    {
      BestOdds = new Dictionary<Outcome, double>();
      //url
      MatchURL = new Uri("http://odds.bestbetting.com" + PartURL);
      
      //Best Odds
      var oddsTokens = WebUtils.ParseWebsite<BestBettingScheduleMatchOdds>(BestOddsString, s => Console.WriteLine(s))
                               .Cast<BestBettingScheduleMatchOdds>();

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

      //payout
      Payout = Convert.ToDouble(PayoutString);
    }


  }
}
