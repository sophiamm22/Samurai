﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Domain.Entities;

namespace Samurai.Domain.Model
{

  public class GenericPrediction
  {
    public GenericPrediction()
    {
      OutcomeProbabilities = new Dictionary<Outcome, double>();
      ScoreLineProbabilities = new Dictionary<string, double?>();
    }
    public int MatchID { get; set; }
    public string MatchIdentifier { get; set; }

    public string Sport { get; set; }
    public string TournamentName { get; set; }
    public string TournamentEventName { get; set; }
    public Uri PredictionURL { get; set; }

    public DateTime MatchDate { get; set; }

    public string TeamOrPlayerA { get; set; }
    public string TeamOrPlayerB { get; set; }

    public string PlayerAFirstName { get; set; }
    public string PlayerBFirstName { get; set; }

    public IDictionary<Outcome, double> OutcomeProbabilities { get; set; }
    public IDictionary<string, double?> ScoreLineProbabilities { get; set; }

  }

}
