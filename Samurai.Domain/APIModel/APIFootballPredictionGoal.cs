using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;


namespace Samurai.Domain.APIModel
{
  [JsonObject]
  public class APIFootballPredictionGoal
  {
    [JsonProperty("homeGoal")]
    public int HomeGoals { get; set; }
    [JsonProperty("awayGoal")]
    public int AwayGoals { get; set; }
    [JsonProperty("chance")]
    public double Probability { get; set; }
  }
}
