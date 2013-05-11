using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Samurai.Core;

namespace Samurai.Domain.APIModel
{
  [JsonObject]
  public class APIFootballPredictionChance
  {
    [JsonProperty("homewin")]
    public double HomeWinProb { get; set; }
    [JsonProperty("draw")]
    public double DrawProb { get; set; }
    [JsonProperty("awaywin")]
    public double AwayWinProb { get; set; }
  }
}
