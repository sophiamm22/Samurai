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
    public string PredictionURL { get; set; }
    public DateTime MatchDate { get; set; }
    public OutcomeProbabilityViewModel PlayerAProbability { get; set; }
    public OutcomeProbabilityViewModel PlayerBProbability { get; set; }

    public IEnumerable<ScoreLineProbabilityViewModel> ScoreLineProbabilities { get; set; }
  }
}
