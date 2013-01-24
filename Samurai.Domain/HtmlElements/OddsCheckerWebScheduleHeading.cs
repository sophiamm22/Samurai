﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Samurai.Core;
using Samurai.Domain.Model;

namespace Samurai.Domain.HtmlElements
{
  public class OddsCheckerWebScheduleHeading : IRegexableWebsite
  {
    public string Heading { get; set; }
    

    public int Identifier { get; set; }
    public List<Regex> Regexs
    {
      get
      {
        return new List<Regex>()
        {
          
          new Regex(@"\<h2 class=ætitleæ\>(?<Heading>[^\<]+)\</h2\>")
        };
      }
    }

    public bool Validates() { return true; }
    public void Clean()
    {
      Heading = Heading.Trim();
    }
  }  
}
