using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;
using Samurai.Domain.Model;

namespace Samurai.Domain.HtmlElements
{
  public class OddsCheckerWebScheduleDate : IRegexableWebsite
  {
    public string DateString { get; set; }
    
    public DateTime ScheduleDate { get; set; }

    public int Identifier { get; set; }
    public List<Regex> Regexs
    {
      get
      {
        return new List<Regex>()
        {
          // deprecated - new Regex(@"\<td colspan=æ6æ\>(?<DateString>\d{1,2} [A-Za-z]+ \d{4})\</td\>"),
          new Regex(@"\<td class=ædayæ colspan=æ5æ\>[^\<]+\<p\>(?<DateString>\d{1,2} [A-Za-z]+ \d{4})\</p\>[^\<]+\</td\>")
        };
      }
    }

    public bool Validates() { return true; }
    public void Clean()
    {
      DateTime dateOut;
      if (DateTime.TryParse(DateString, out dateOut))
        ScheduleDate = dateOut;
    }
  }
}
