using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Samurai.Core
{
  public interface IRegexableWebsite
  {
    int Identifier { get; set; }
    List<Regex> Regexs { get; }
    bool Validates();
    void Clean();
  }
}
