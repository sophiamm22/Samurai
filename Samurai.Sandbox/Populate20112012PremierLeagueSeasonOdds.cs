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
  public class Populate20112012PremierLeagueSeasonOdds
  {
    private readonly IWindsorContainer container;
    private List<FootballFixtureViewModel> fixtures;

    public Populate20112012PremierLeagueSeasonOdds(IWindsorContainer container)
    {
      if (container == null) throw new ArgumentNullException("container");
      this.container = container;
      this.fixtures = new List<FootballFixtureViewModel>();
    }

    public void Populate()
    {
      GetFixtures();
      GetPredictions();
      GetOdds();
    }

    private void GetFixtures()
    {
      var spreadsheetData = this.container.Resolve<ISpreadsheetData>();
      spreadsheetData.ReadData();

      var fixtureService = this.container.Resolve<IFootballFixtureService>();

      var dates = Enumerable.Range(0, 280).Select(d => new DateTime(2011, 08, 13).AddDays(d));

      foreach (var date in dates)
      {
        spreadsheetData.CouponDate = date;

        var fixtures = fixtureService.FetchSkySportsFootballResults(date)
                                     .ToList();
        if (fixtures.Count == 0)
          Console.WriteLine(string.Format("No fixtures on {0}", date.ToShortDateString()));
        else
        {
          this.fixtures.AddRange(fixtures);
          Console.WriteLine(string.Format("Fixtures on {0}:", date.ToShortDateString()));
          fixtures.ForEach(f => Console.WriteLine(f.ToString()));
        }
      }
    }

    private void GetPredictions()
    {
      var predictionService = this.container.Resolve<IFootballPredictionService>();
      var matches = predictionService.FetchFootballPredictions(this.fixtures);
    }

    private void GetOdds()
    {
      var oddsService = this.container.Resolve<IFootballOddsService>();
      var spreadsheetData = this.container.Resolve<ISpreadsheetData>();

      var dates = this.fixtures.Select(f => f.MatchDate.Date).Distinct().ToList();
      foreach (var date in dates)
      {
        spreadsheetData.CouponDate = date;
        oddsService.FetchAllFootballOdds(date);
      }
      
    }
  }
}
