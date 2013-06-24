using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;

using Samurai.Web.API.App_Start;
using Samurai.Services.AutoMapper;

namespace Samurai.Web.API
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class WebApiApplication : System.Web.HttpApplication
  {
    protected void Application_Start()
    {
      AutoMapperManualConfiguration.Configure();
      IOCConfig.RegisterIOC();
      RouteTable.Routes.MapHubs(new HubConfiguration() { EnableCrossDomain = true });
      WebApiConfig.Register(GlobalConfiguration.Configuration);
      
    }
  }
}