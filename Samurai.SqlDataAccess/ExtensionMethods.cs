using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Samurai.SqlDataAccess
{
  public static class ExtensionMethods
  {
    public static string RemoveSpecialCharacters(this string input)
    {
      Regex r = new Regex("(?:[^a-z0-9 -]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
      return r.Replace(input, String.Empty);
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
