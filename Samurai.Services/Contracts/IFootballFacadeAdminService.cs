﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels.Football;

namespace Samurai.Services.Contracts
{
  public interface IFootballFacadeAdminService
  {
    void AddAlias(string source, string playerName, string valueSamuraiName);
    IEnumerable<FootballFixtureViewModel> UpdateDaysSchedule(DateTime fixtureDate);
    IEnumerable<FootballFixtureViewModel> UpdateDaysResults(DateTime fixtureDate);
    IEnumerable<FootballLadderViewModel> GetTournamentLadder(DateTime matchDate, string tournament);
  }
}