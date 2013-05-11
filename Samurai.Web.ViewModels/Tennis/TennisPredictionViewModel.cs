using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Samurai.Web.ViewModels.Value;

namespace Samurai.Web.ViewModels.Tennis
{
  public class TennisPredictionViewModel
  {
    public string MatchIdentifier { get; set; }
    public int MatchId { get; set; }
    public string PredictionURL { get; set; }
    public DateTime MatchDate { get; set; }

    public int PlayerAGames { get; set; }
    public int PlayerBGames { get; set; }

    public double? EPoints { get; set; }
    public double? EGames { get; set; }
    public double? ESets { get; set; }

    public Dictionary<string, double> Probabilities { get; set; }
    
    public IEnumerable<ScoreLineProbabilityViewModel> ScoreLineProbabilities { get; set; }
  }
}
