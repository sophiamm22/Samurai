﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Model
{
  public class GenericMatchDetail
  {
    public string MatchIdentifier { get; set; }

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
    public string ObservedOutcome { get; set; }

    public int? IKTSGameWeek { get; set; }

  }
}