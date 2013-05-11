using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Samurai.Core
{
  public interface IReadableImage
  {
    Guid FileGuid { get; set; }
    List<Regex> Regexs { get; set; }
    Func<string, string> StringManipulation { get; set; }
  }
}
