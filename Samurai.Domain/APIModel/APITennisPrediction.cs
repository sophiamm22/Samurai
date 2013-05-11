using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Newtonsoft.Json;

using Samurai.Core;

namespace Samurai.Domain.APIModel
{
  [JsonObject]
  [Serializable]
  public class APITennisPrediction : IRegexableWebsite
  {
    public int Identifier { get; set; }
    public List<Regex> Regexs { get; set; }
    public bool Validates() { return true; }
    public void Clean() { }

    public DateTime StartTime { get; set; }

    [JsonProperty]
    public string PlayerAFullName { get; set; }
    [JsonProperty]
    public string PlayerAFirstname { get; set; }
    [JsonProperty]
    public string PlayerASurname { get; set; }
    [JsonProperty]
    public string PlayerBFullName { get; set; }
    [JsonProperty]
    public string PlayerBFirstname { get; set; }
    [JsonProperty]
    public string PlayerBSurname { get; set; }
    [JsonProperty]
    public string TournamentName { get; set; }
    [JsonProperty]
    public int Year { get; set; }
    [JsonProperty]
    public string Round { get; set; }
    [JsonProperty]
    public string Surface { get; set; }

    [JsonProperty]
    public double PlayerAProbability { get; set; }
    [JsonProperty]
    public double PlayerBProbability { get; set; }
    
    [JsonProperty]
    public bool FiveSets { get; set; }

    [JsonProperty]
    public double? ProbThreeLove { get; set; }
    [JsonProperty]
    public double? ProbThreeOne { get; set; }
    [JsonProperty]
    public double? ProbThreeTwo { get; set; }
    [JsonProperty]
    public double? ProbTwoThree { get; set; }
    [JsonProperty]
    public double? ProbOneThree { get; set; }
    [JsonProperty]
    public double? ProbLoveThree { get; set; }

    [JsonProperty]
    public double? ProbTwoLove { get; set; }
    [JsonProperty]
    public double? ProbTwoOne { get; set; }
    [JsonProperty]
    public double? ProbOneTwo { get; set; }
    [JsonProperty]
    public double? ProbLoveTwo { get; set; }
    
    [JsonProperty]
    public double? ExpectedPoints { get; set; }
    [JsonProperty]
    public double? ExpectedGames { get; set; }
    [JsonProperty]
    public double? ExpectedSets { get; set; }

    [JsonProperty]
    public int PlayerAGames { get; set; }
    [JsonProperty]
    public int PlayerBGames { get; set; }

    public static string CSVHeaders()
    {
      return "Player A Full Name,Player A First Name,Player A Surname,Player B Full Name,Player B First Name,Player B Surname,Tournament Name,Year,Round,Surface,Player A Probability,Player B Probability,Five Sets?,Prob 3-0,Prob 3-1,Prob 3-2,Prob 2-3,Prob 1-3,Prob 0-3,Prob 2-0,Prob 2-1,Prob 1-2,Prob 0-2,Expected Points,Expected Games,Expected Sets,Player A Games,Player B Games";
    }

    public string CSVLine()
    {
      const string c = ",";
      var line =
        '\"' + PlayerAFullName + '\"' + c +
        PlayerAFirstname + c +
        PlayerASurname + c +
        '\"' + PlayerBFullName + '\"' + c +
        PlayerBFirstname + c +
        PlayerBSurname + c +
        TournamentName + c +
        Year.ToString() + c +
        Round + c +
        Surface + c +
        PlayerAProbability.ToString() + c +
        PlayerBProbability.ToString() + c +
        FiveSets.ToString() + c +
        (ProbThreeLove.HasValue ? ProbThreeLove.ToString() : "") + c +
        (ProbThreeOne.HasValue ? ProbThreeOne.ToString() : "") + c +
        (ProbThreeTwo.HasValue ? ProbThreeTwo.ToString() : "") + c +
        (ProbTwoThree.HasValue ? ProbTwoThree.ToString() : "") + c +
        (ProbOneThree.HasValue ? ProbOneThree.ToString() : "") + c +
        (ProbLoveThree.HasValue ? ProbLoveThree.ToString() : "") + c +
        (ProbTwoLove.HasValue ? ProbTwoLove.ToString() : "") + c +
        (ProbTwoOne.HasValue ? ProbTwoOne.ToString() : "") + c +
        (ProbOneTwo.HasValue ? ProbOneTwo.ToString() : "") + c +
        (ProbLoveTwo.HasValue ? ProbLoveTwo.ToString() : "") + c +
        (ExpectedPoints.HasValue ? ExpectedPoints.ToString() : "") + c +
        (ExpectedGames.HasValue ? ExpectedGames.ToString() : "") + c +
        (ExpectedSets.HasValue ? ExpectedSets.ToString() : "") + c +
        (PlayerAGames.ToString()) + c +
        (PlayerBGames.ToString());

      return line;
    }

  }
}
