using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Castle.Windsor;
using Samurai.Web.Windsor;

namespace Samurai.Web
{
  public static class IOCConfig
  {
    public static IWindsorContainer RegisterIOC()
    {
      var container = new WindsorContainer();

      container.Install(new SamuraiWindsorInstaller());
      
      var controllerFactory = new WindsorControllerFactory(container);
      ControllerBuilder.Current.SetControllerFactory(controllerFactory);

      return container;
    }
  }
}