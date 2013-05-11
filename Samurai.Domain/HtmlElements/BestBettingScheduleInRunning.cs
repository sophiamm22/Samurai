using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;
using Samurai.Domain.Model;

namespace Samurai.Domain.HtmlElements
{
  public class BestBettingScheduleInRunning : IRegexableWebsite
  {
    public int Identifier { get; set; }
    public List<Regex> Regexs
    {
      get
      {
        var regexs = new List<Regex>();
        regexs.Add(new Regex(@"inrunning\.gif"));
        return regexs;
      }
    }

    public bool Validates() { return true; }
    public void Clean()
    {

    }


  }
}
