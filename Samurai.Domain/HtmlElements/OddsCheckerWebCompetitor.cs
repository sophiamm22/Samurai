using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;

namespace Samurai.Domain.HtmlElements
{
  public class OddsCheckerWebCompetitor : IRegexableWebsite
  {
    public int Identifier { get; set; }

    public string OutcomeFullName { get; set; }

    public string Outcome { get; set; }

    public List<Regex> Regexs
    {
      get
      {
        return new List<Regex>()
        {
          new Regex(@"\<a class=æpopup selTxtæ [^\>]+\>(?<OutcomeFullName>[^\<]+)\</a\>")
        };
      }
    }

    public bool Validates() { return true; }
    public void Clean()
    {
      Outcome = OutcomeFullName.Trim().Replace("&amp;", "&");
    }

  }
}
