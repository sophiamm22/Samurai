﻿using System;
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
    public int MatchId { get; set; }
    public string PredictionURL { get; set; }

    public Dictionary<string, double> Probabilities { get; set; }

    public IEnumerable<ScoreLineProbabilityViewModel> ScoreLineProbabilities { get; set; }
  }
}
