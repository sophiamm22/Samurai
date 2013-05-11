using System;
using System.Web.Optimization;

[assembly: WebActivator.PostApplicationStartMethod(
    typeof(Samurai.Web.Client.App_Start.AppConfig), "PreStart")]

namespace Samurai.Web.Client.App_Start
{
    public static class AppConfig
    {
        public static void PreStart()
        {
            // Add your start logic here
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}