using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;

using Samurai.Services.Contracts;
using Samurai.Domain.Value.Excel;
using Samurai.Domain.Model;
using Samurai.Domain.Entities;
using Samurai.Web.ViewModels;

namespace Samurai.Sandbox
{
  public class Populate2012TennisSeasonOdds
  {
    private readonly IWindsorContainer container;

    public Populate2012TennisSeasonOdds(IWindsorContainer container)
    {
      if (container == null) throw new ArgumentNullException("container");
      this.container = container;
    }

    public void Populate()
    {
      var spreadsheetData = this.container.Resolve<IFootballSpreadsheetData>();
      spreadsheetData.ReadData();

      var dates = Enumerable.Range(0, 365)
                            .Select(d => new DateTime(2012, 01, 01).AddDays(d))
                            .ToList();

      foreach (var date in dates)
      {
        spreadsheetData.CouponDate = date;
        GetFixtures(date);
        GetPredictions(date);
        GetOdds(date);        
      }
    }

    private void GetFixtures(DateTime date)
    {
      var fixtureService = this.container.Resolve<ITennisFixtureAdminService>();
      fixtureService.FetchTennisResults(date);
    }

    private void GetPredictions(DateTime date)
    {
      var predictionService = this.container.Resolve<ITennisPredictionAdminService>();
      var predictions = predictionService.FetchTennisPredictions(date);
    }

    private void GetOdds(DateTime date)
    {
      var oddsService = this.container.Resolve<ITennisOddsAdminService>();
      var fullFixtureDetails = oddsService.FetchAllTennisOdds(date);
    }

  }
}
