using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using Samurai.Core;

namespace Samurai.Domain.APIModel
{
  public class APIFootballPrediction : IRegexableWebsite
  {
    public int Identifier { get; set; }
    public List<Regex> Regexs { get; set; }
    public void Clean()
    {
      throw new NotImplementedException();
    }
    public bool Validates()
    {
      return true;
    }

    [JsonProperty("Sta")]
    public string Status { get; set; }
    [JsonProperty("NameH")]
    public string HomeTeam { get; set; }
    [JsonProperty("NameA")]
    public string AwayTeam { get; set; }
    [JsonProperty("ExpGD")]
    public double ExpectedGoalDifference { get; set; }
    [JsonProperty("gameType")]
    public int GameType { get; set; }
    [JsonProperty("ExpPt")]
    public APIFootballPredicitonExpectedPoint ExpectedPoints { get; set; }
    [JsonProperty("chances")]
    public APIFootballPredictionChance ExpectedProbabilities { get; set; }
    [JsonProperty("goals")]
    public IEnumerable<APIFootballPredictionGoal> ScoreProbabilities { get; set; }


  }
}
