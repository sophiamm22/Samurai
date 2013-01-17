using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using Samurai.Core;

namespace Samurai.Domain.APIModel
{
  public class APITennisTourCalendar : IRegexableWebsite
  {
    public int Identifier { get; set; }
    public List<Regex> Regexs { get; set; }
    public bool Validates() { return true; }
    public void Clean() { }

    [JsonProperty]
    public string TournamentName { get; set; }
    [JsonProperty]
    public DateTime StartDate { get; set; }
    [JsonProperty]
    public DateTime EndDate { get; set; }
    [JsonProperty]
    public int Tour { get; set; }
    [JsonProperty]
    public int Surface { get; set; }
    [JsonProperty]
    public int Draw { get; set; }
    [JsonProperty]
    public int GamesCount { get; set; }
    [JsonProperty]
    public bool Success { get; set; }
    [JsonProperty]
    public bool InProgress { get; set; }
    [JsonProperty]
    public bool Completed { get; set; }
  }
}
