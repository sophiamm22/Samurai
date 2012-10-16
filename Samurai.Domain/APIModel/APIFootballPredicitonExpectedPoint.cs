using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace Samurai.Domain.APIModel
{
  [JsonObject]
  public class APIFootballPredicitonExpectedPoint
  {
    [JsonProperty("home")]
    public double Home { get; set; }
    [JsonProperty("away")]
    public double Away { get; set; }
    [JsonProperty("diff")]
    public double Difference { get; set; }
  }

}
