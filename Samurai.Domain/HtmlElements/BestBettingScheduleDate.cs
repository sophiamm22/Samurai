using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;
using Samurai.Domain.Model;

namespace Samurai.Domain.HtmlElements
{
  public class BestBettingScheduleDate : IRegexableWebsite
  {
    public string ScheduleDateString { get; set; }
    
    public DateTime ScheduleDate { get; set; }

    public int Identifier { get; set; }
    public List<Regex> Regexs
    {
      get
      {
        var regexs = new List<Regex>();
        regexs.Add(new Regex(@"\<th class=æfirstColumnæ\>(?<ScheduleDateString>[^\<]+)\<"));
        return regexs;
      }
    }

    public bool Validates() { return true; }
    public void Clean()
    {
      DateTime date = DateTime.Now;
      if (DateTime.TryParse(ScheduleDateString, out date))
        ScheduleDate = date;
    }

    public override string ToString()
    {
      return ScheduleDate.ToShortDateString();
    }

  }
}
