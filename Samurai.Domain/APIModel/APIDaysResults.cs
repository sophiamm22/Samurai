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
  public class APIDaysResults : IRegexableWebsite
  {
    public int Identifier { get; set; }
    public List<Regex> Regexs { get; set; }
    public bool Validates() { return true; }
    public void Clean() { }

    [JsonProperty]
    public string TournamentName { get; set; }
    [JsonProperty]
    public int BestOfSets { get; set; }
    [JsonProperty]
    public DateTime GameDate { get; set; }
    [JsonProperty]
    public DateTime RoundDate { get; set; }
    [JsonProperty]
    public int RoundID { get; set; }
    [JsonProperty]
    public string Round { get; set; }
    //[JsonProperty]
    //public DateTime Time { get; set; }
    [JsonProperty]
    public string Comment { get; set; }
    [JsonProperty]
    public bool LoserRetired { get; set; }
    [JsonProperty]
    public bool LoserWalkedOver { get; set; }
    [JsonProperty]
    public string Score { get; set; }

    [JsonProperty]
    public string WinnerSurname { get; set; }
    [JsonProperty]
    public string WinnerFirstName { get; set; }
    
    [JsonProperty]
    public string LoserSurname { get; set; }
    [JsonProperty]
    public string LoserFirstName { get; set; }

    [JsonProperty]
    public int? WinnerFirstSetScore { get; set; }
    [JsonProperty]
    public int? WinnerSecondSetScore { get; set; }
    [JsonProperty]
    public int? WinnerThirdSetScore { get; set; }
    [JsonProperty]
    public int? WinnerFourthSetScore { get; set; }
    [JsonProperty]
    public int? WinnerFifthSetScore { get; set; }

    [JsonProperty]
    public int? WinnerFirstTieBreakScore { get; set; }
    [JsonProperty]
    public int? WinnerSecondTieBreakScore { get; set; }
    [JsonProperty]
    public int? WinnerThirdTieBreakScore { get; set; }
    [JsonProperty]
    public int? WinnerFourthTieBreakScore { get; set; }
    [JsonProperty]
    public int? WinnerFifthTieBreakScore { get; set; }

    [JsonProperty]
    public int? LoserFirstSetScore { get; set; }
    [JsonProperty]
    public int? LoserSecondSetScore { get; set; }
    [JsonProperty]
    public int? LoserThirdSetScore { get; set; }
    [JsonProperty]
    public int? LoserFourthSetScore { get; set; }
    [JsonProperty]
    public int? LoserFifthSetScore { get; set; }

    [JsonProperty]
    public int? LoserFirstTieBreakScore { get; set; }
    [JsonProperty]
    public int? LoserSecondTieBreakScore { get; set; }
    [JsonProperty]
    public int? LoserThirdTieBreakScore { get; set; }
    [JsonProperty]
    public int? LoserFourthTieBreakScore { get; set; }
    [JsonProperty]
    public int? LoserFifthTieBreakScore { get; set; }

  }
}
