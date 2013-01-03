﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Castle.Windsor;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Tennis;

namespace Samurai.Sandbox
{
  public class FullTennisDownload
  {
    private readonly IWindsorContainer container;
    private readonly DateTime date;

    public IEnumerable<TennisFixtureViewModel> Fixtures { get; set; }

    public FullTennisDownload(IWindsorContainer container, DateTime date)
    {
      if (container == null) throw new ArgumentNullException("container");
      if (date == null) throw new ArgumentNullException("date");

      this.container = container;
      this.date = date;
    }

    public void PopulateDatabaseNew()
    {
      var tennisService = this.container.Resolve<ITennisFacadeService>();
      Fixtures = tennisService.UpdateDaysSchedule(this.date);
    }

    public void PopulateDatabase()
    {
      GetPredictions();
      GetOdds();
    }

    private void GetPredictions()
    {
      var predictionService = this.container.Resolve<ITennisPredictionService>();
      var predictions = predictionService.FetchTennisPredictions(this.date);
    }

    private void GetOdds()
    {
      var oddsService = this.container.Resolve<ITennisOddsService>();
      var fullFixtureDetails = oddsService.FetchAllTennisOdds(this.date);
    }
  }
}
