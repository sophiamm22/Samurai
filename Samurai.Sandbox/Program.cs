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
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.Sandbox
{
  class Program
  {
    static void Main(string[] args)
    {
      AutoMapperManualConfiguration.Configure();
      var container = new WindsorContainer();
      container.Install(new SamuraiSandboxWindsorInstaller());

      var footballService = container.Resolve<IFootballFacadeService>();
      var tennisService = container.Resolve<ITennisFacadeService>();

      var samuraiConsole = new SamuraiConsole(footballService, tennisService);
      samuraiConsole.SamuraiMenu();

    }
  }
}
