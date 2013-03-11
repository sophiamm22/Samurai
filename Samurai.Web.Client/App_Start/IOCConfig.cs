using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;

using Castle.Windsor;

using Samurai.Web.Client.Windsor;

namespace Samurai.Web.Client.App_Start
{
  public static class IOCConfig
  {
    public static void RegisterIOC()
    {
      var container = new WindsorContainer();
      container.Install(new SamuraiWebWindsorInstaller());

      GlobalConfiguration.Configuration.Services.Replace(
        typeof(IHttpControllerActivator),
        new WindsorAPIControllerActivator(container));

    }
  }
}