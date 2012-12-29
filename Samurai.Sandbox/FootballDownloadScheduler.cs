using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Castle.Windsor;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;

namespace Samurai.Sandbox
{
  public class FootballDownloadScheduler
  {
    private readonly IWindsorContainer container;
    private readonly DateTime date;
    private readonly IFootballFixtureService footballFixtureService;
    private readonly IFootballPredictionService footballPredictionService;
    private readonly IFootballOddsService footballOddsService;

    public FootballDownloadScheduler(IWindsorContainer container, DateTime date)
    {
      if (container == null) throw new ArgumentNullException("container");
      if (date == null) throw new ArgumentNullException("date");

      this.container = container;
      this.date = date;
      this.footballFixtureService = this.container.Resolve<IFootballFixtureService>();
      this.footballPredictionService = this.container.Resolve<IFootballPredictionService>();
      this.footballOddsService = this.container.Resolve<IFootballOddsService>();
    }

    public void RunSchedule()
    {

    }

  }
}
