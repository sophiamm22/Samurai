using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;

namespace Samurai.Domain.HtmlElements
{
  public class OddsCheckerWebCard : IRegexableWebsite
  {
    public string CardID { get; set; }

    public int Identifier { get; set; }

    public List<Regex> Regexs
    {
      get
      {
        return new List<Regex>()
        {
          new Regex(@"oc\.cardId = æ(?<CardID>[^æ]+)æ")
        };
      }
    }
    public bool Validates() { return true; }
    public void Clean()
    {
      return;
    }
   
  }
}
