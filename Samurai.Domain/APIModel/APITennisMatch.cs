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
  
  public class APITennisMatch : IRegexableWebsite
  {
    
    public int Identifier { get; set; }
    
    public List<Regex> Regexs { get; set; }

    public bool Validates() { return true; }

    public void Clean() { }

    [JsonProperty]
    
    public string PlayerAFirstName { get; set; }
    [JsonProperty]
    
    public string PlayerASurname { get; set; }
    [JsonProperty]
    
    public string PlayerAName { get; set; }
    [JsonProperty]
    
    public string PlayerBFirstName { get; set; }
    [JsonProperty]
    
    public string PlayerBSurname { get; set; }
    [JsonProperty]
    
    public string PlayerBName { get; set; }
    [JsonProperty]
    
    public string TournamentName { get; set; }
    [JsonProperty]
    
    public DateTime MatchDate { get; set; }

    public override string ToString()
    {
      return string.Format("http://www.tennisbetting365.com/api/getprediction/{0}/{1}/{2}/{3}/vs/{4}/{5}",
        TournamentName.ToHyphenated(), MatchDate.Year.ToString().ToHyphenated(), 
        PlayerAFirstName.ToHyphenated(), PlayerASurname.ToHyphenated(),
        PlayerBFirstName.ToHyphenated(), PlayerBSurname.ToHyphenated());
    }

  }
}
