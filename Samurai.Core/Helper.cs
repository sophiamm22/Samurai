using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Samurai.Core
{
  public static class Helper
  {
    public static string ToHyphenated(this string text)
    {
      var rgx = new Regex("[^a-zA-Z0-9_ -]");
      var oStr = text.RemoveDiacritics().Replace("ô", "o").Replace("é", "e").Replace("ü", "u");
      var str = rgx.Replace(oStr, "").Replace(" ", "-").ToLower();
      return str;
    }

    public static string RemoveDiacritics(this string value)
    {
      if (String.IsNullOrEmpty(value))
        return value;

      string normalized = value.Normalize(NormalizationForm.FormD);
      StringBuilder sb = new StringBuilder();

      foreach (char c in normalized)
      {
        if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
          sb.Append(c);
      }

      Encoding nonunicode = Encoding.GetEncoding(850);
      Encoding unicode = Encoding.Unicode;

      byte[] nonunicodeBytes = Encoding.Convert(unicode, nonunicode, unicode.GetBytes(sb.ToString()));
      char[] nonunicodeChars = new char[nonunicode.GetCharCount(nonunicodeBytes, 0, nonunicodeBytes.Length)];
      nonunicode.GetChars(nonunicodeBytes, 0, nonunicodeBytes.Length, nonunicodeChars, 0);

      return new string(nonunicodeChars);
    }

  }

}
