using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Samurai.Web.ViewModels.Value;


namespace Samurai.Web.ViewModels.Football
{
  public class FootballPredictionViewModel
  {
    public string MatchIdentifier { get; set; }
    public string PredictionURL { get; set; }
    public DateTime MatchDate { get; set; }

    public OutcomeProbabilityViewModel HomeWinProbability { get; set; }
    public OutcomeProbabilityViewModel DrawProbabiltity { get; set; }
    public OutcomeProbabilityViewModel AwayWinProbability { get; set; }

    public IEnumerable<ScoreLineProbabilityViewModel> ScoreLineProbabilities { get; set; }
  }
}
