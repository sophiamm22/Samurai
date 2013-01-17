using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Castle.Windsor;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;

namespace Samurai.Sandbox
{
  public class FullFootballDownload
  {
    private readonly IWindsorContainer container;
    private readonly DateTime date;

    public IEnumerable<FootballFixtureViewModel> Fixtures { get; set; }

    public FullFootballDownload(IWindsorContainer container, DateTime date)
    {
      if (container == null) throw new ArgumentNullException("container");
      if (date == null) throw new ArgumentNullException("date");

      this.container = container;
      this.date = date;
    }

    public void PopulateDatabaseNew()
    {
      var footballService = this.container.Resolve<IFootballFacadeService>();
      Fixtures = footballService.UpdateDaysSchedule(this.date);
    }

    public void PopulateDatabase()
    {
      GetFixtures();
      GetPredictions();
      GetOdds();
    }

    private void GetFixtures()
    {
      var fixtureService = this.container.Resolve<IFootballFixtureService>();
      this.Fixtures = fixtureService.FetchSkySportsFootballFixturesNew(this.date);
    }

    private void GetPredictions()
    {
      var predictionService = this.container.Resolve<IFootballPredictionService>();
      var predictions = predictionService.FetchFootballPredictions(this.Fixtures);
    }

    private void GetOdds()
    {
      var oddsService = this.container.Resolve<IFootballOddsService>();

      var fullFixtureDetail = oddsService.FetchAllFootballOdds(this.date);

    }
  }
}
