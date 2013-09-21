﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Web.ViewModels.Value;

namespace Samurai.Web.ViewModels.Football
{
  public class FootballFixtureViewModel
  {
    public string MatchIdentifier { get; set; }
    public int Id { get; set; }
    public string League { get; set; }
    public string Season { get; set; }
    public DateTime MatchDate { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
    public string ScoreLine { get; set; }
    public int? IKTSGameWeek { get; set; }

    public FootballPredictionViewModel Predictions { get; set; }
    public IEnumerable<OddViewModel> Odds { get; set; }

  }
}
