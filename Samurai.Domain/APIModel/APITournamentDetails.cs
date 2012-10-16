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
  public class APITournamentDetails : IRegexableWebsite
  {
    public int Identifier { get; set; }
    public List<Regex> Regexs { get; set; }
    public bool Validates() { return true; }
    public void Clean() { }

    [JsonProperty]
    public List<APITournamentDetail> TournamentDetails { get; set; }
  }

  [JsonObject]
  public class APITournamentDetail : IRegexableWebsite
  {
    public int Identifier { get; set; }
    public List<Regex> Regexs { get; set; }
    public bool Validates() { return true; }
    public void Clean() { }

    [JsonProperty]
    public string APIMessage { get; set; }
    [JsonProperty]
    public string TournamentName { get; set; }
    [JsonProperty]
    public DateTime StartDate { get; set; }
    [JsonProperty]
    public string Surface { get; set; }
    [JsonProperty]
    public int Draw { get; set; }
    [JsonProperty]
    public bool Completed { get; set; }
    [JsonProperty]
    public bool InProgress { get; set; }
    [JsonProperty]
    public List<APITournamentLadder> TournamentLadders { get; set; }

  }
}
