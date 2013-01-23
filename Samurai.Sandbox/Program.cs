using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;

using Samurai.Services.Contracts;
using Samurai.Services.AutoMapper;
using Samurai.Domain.Exceptions;
using Samurai.Web.ViewModels.Value;

namespace Samurai.Sandbox
{
  class Program
  {
    static void Main(string[] args)
    {
      AutoMapperManualConfiguration.Configure();
      var container = new WindsorContainer();
      container.Install(new SamuraiSandboxWindsorInstaller());

      var tennisService = container.Resolve<ITennisFacadeService>();
      var testNewTennisServiceFacade = new FullTennisDownload(tennisService, new DateTime(2013, 01, 23));
      //testNewTennisServiceFacade.Get2013Calendar();
      testNewTennisServiceFacade.PopulateDatabaseNew();

      //var testNewFootballServiceFacade = new FullFootballDownload(container, new DateTime(2013, 01, 02));
      //testNewFootballServiceFacade.PopulateDatabaseNew();

      //var didItBaby = new Populate2012TennisSeasonOdds(container);
      //didItBaby.Populate();

      //var diditbaby = new Populate2011PremierLeagueSeasonOdds(container);
      //diditbaby.Populate();

      //var date = new DateTime(2012, 08, 17);

      //var doitbaby = new FullTennisDownload(container, date);
      //doitbaby.PopulateDatabase();
    }
  }
}
