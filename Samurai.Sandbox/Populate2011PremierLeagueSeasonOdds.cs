using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Samurai.Services.Contracts;

namespace Samurai.Sandbox
{
  public class Populate2011PremierLeagueSeasonOdds
  {
    private readonly IWindsorContainer container;

    public Populate2011PremierLeagueSeasonOdds(IWindsorContainer container)
    {
      if (container == null) throw new ArgumentNullException("container");
      this.container = container;
    }

    public void Populate()
    {
      GetFixtures();
      GetCoupons();
    }

    private void GetFixtures()
    {
      var fixtureService = this.container.Resolve<IFixtureService>();

      var dates = Enumerable.Range(0, 280).Select(d => new DateTime(2011, 08, 13).AddDays(d));

      foreach (var date in dates)
      {
        var fixtures = fixtureService.FetchSkySportsFootballResults(date)
                                     .ToList();
        if (fixtures.Count == 0)
          Console.WriteLine(string.Format("No fixtures on {0}", date.ToShortDateString()));
        else
        {
          Console.WriteLine(string.Format("Fixtures on {1}:", date.ToShortDateString()));
          fixtures.ForEach(f => Console.WriteLine(f.ToString()));
        }
      }
    }

    private void GetCoupons()
    {
    }
  }
}
