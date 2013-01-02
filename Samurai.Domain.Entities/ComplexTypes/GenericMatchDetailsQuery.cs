using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Entities.ComplexTypes
{
  public class GenericMatchDetailQuery
  {
    public int MatchID { get; set; }
    public DateTime MatchDate { get; set; }

    public int TournamentID { get; set; }
    public string TournamentName { get; set; }
    public int TournamentEventID { get; set; }
    public string TournamentEventName { get; set; }

    public int CompetitionID { get; set; }
    public string CompetitionName { get; set; }

    public int PlayerAID { get; set; }
    public string TeamOrPlayerA { get; set; }
    public string PlayerAFirstName { get; set; }

    public int PlayerBID { get; set; }
    public string TeamOrPlayerB { get; set; }
    public string PlayerBFirstName { get; set; }

    public int? ScoreOutcomeID { get; set; }
    public string ObservedOutcome 
    {
      get
      {
        return (string.IsNullOrEmpty(_scoreAHackString) || string.IsNullOrEmpty(_scoreBHackString)) ? "not played" : string.Format("{0}-{1}", _scoreAHackString.ToString(), _scoreBHackString.ToString());
      }
    }

    public int? IKTSGameWeek { get; set; }

    private string _scoreAHackString;
    public int ScoreAHack
    {
      set
      {
        _scoreAHackString = value == -1 ? string.Empty : value.ToString();
      }
    }

    private string _scoreBHackString;
    public int ScoreBHack
    {
      set
      {
        _scoreBHackString = value == -1 ? string.Empty : value.ToString();
      }
    }

  }
}
