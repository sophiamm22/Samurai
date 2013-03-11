﻿using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using Samurai.Web.Client.App_Start;
using Samurai.Services.AutoMapper;

 namespace Samurai.Web.Client
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : System.Web.HttpApplication
  {
    protected void Application_Start()
    {
      AutoMapperManualConfiguration.Configure();
      IOCConfig.RegisterIOC();
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      WebApiConfig.Register(GlobalConfiguration.Configuration);
      BundleConfig.RegisterBundles(BundleTable.Bundles);
    }
  }
}