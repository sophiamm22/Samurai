using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Domain.Entities;

namespace Samurai.Domain.Model
{
  public interface IGenericPrediction
  {
    string Identifier { get; set; }

    string Sport { get; set; }
    string CompetitionName { get; set; }
    Uri PredictionURL { get; set; }

    DateTime MatchDate { get; set; }

    string TeamOrPlayerA { get; set; }
    string TeamOrPlayerB { get; set; }

    IDictionary<Outcome, double> OutcomeProbabilities { get; set; }
    IDictionary<string, double?> ScoreLineProbabilities { get; set; }
  }

  public class GenericPrediction : IGenericPrediction
  {
    public GenericPrediction()
    {
      OutcomeProbabilities = new Dictionary<Outcome, double>();
      ScoreLineProbabilities = new Dictionary<string, double?>();
    }

    public string Identifier { get; set; }

    public string Sport { get; set; }
    public string CompetitionName { get; set; }
    public Uri PredictionURL { get; set; }

    public DateTime MatchDate { get; set; }

    public string TeamOrPlayerA { get; set; }
    public string TeamOrPlayerB { get; set; }

    public IDictionary<Outcome, double> OutcomeProbabilities { get; set; }
    public IDictionary<string, double?> ScoreLineProbabilities { get; set; }

  }

}
