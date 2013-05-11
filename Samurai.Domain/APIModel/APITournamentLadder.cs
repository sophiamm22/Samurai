using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

using Samurai.Core;

namespace Samurai.Domain.APIModel
{
  [JsonObject]
  public class APITournamentLadder : IRegexableWebsite
  {
    public int Identifier { get; set; }
    public List<Regex> Regexs { get; set; }
    public bool Validates() { return true; }
    public void Clean() { }

    [JsonProperty]
    public int Position { get; set; }
    [JsonProperty]
    public bool ByeOrQualifier { get; set; }
    [JsonProperty]
    public string PlayerName { get; set; }
    [JsonProperty]
    public string PlayerFirstName { get; set; }
    [JsonProperty]
    public string PlayerSurname { get; set; }
    [JsonProperty]
    public string PlayerCountry { get; set; }
    [JsonProperty]
    public bool Qualifier { get; set; }
    [JsonProperty]
    public bool LuckyLoser { get; set; }
    [JsonProperty]
    public bool WildCard { get; set; }
    [JsonProperty]
    public int? Seed { get; set; }
  }
}
