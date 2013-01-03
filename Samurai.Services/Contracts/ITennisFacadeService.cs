﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels.Tennis;

namespace Samurai.Services.Contracts
{
  public interface ITennisFacadeService
  {
    IEnumerable<TennisFixtureViewModel> UpdateDaysSchedule(DateTime fixtureDate);
  }
}
