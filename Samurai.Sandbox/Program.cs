using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Samurai.Services.AutoMapper;

namespace Samurai.Sandbox
{
  class Program
  {
    static void Main(string[] args)
    {
      AutoMapperManualConfiguration.Configure();
      var container = new WindsorContainer();
      container.Install(new SamuraiSandboxWindsorInstaller());

      var date = new DateTime(2012, 11, 18);

      //var diditbaby = new Populate2011PremierLeagueSeasonOdds(container);
      //diditbaby.Populate();

      var doitbaby = new FullFootballDownload(container, date);
      doitbaby.PopulateDatabase();
    }
  }
}
