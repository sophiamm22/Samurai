using System;
using System.Web.Optimization;

namespace Samurai.Web.Client
{
  public class BundleConfig
  {
    public static void RegisterBundles(BundleCollection bundles)
    {
      bundles.IgnoreList.Clear();
      AddDefaultIgnorePatterns(bundles.IgnoreList);

      // Modernizr
      bundles.Add(new ScriptBundle("~/bundles/modernizr")
        .Include("~/Scripts/modernizr-{version}.js"));

      // jQuery
      bundles.Add(new ScriptBundle("~/bundles/jquery",
          "//ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js")
          .Include("~/Scripts/jquery-{version}.js"));


      bundles.Add(
        new ScriptBundle("~/scripts/vendor")
          .Include("~/scripts/jquery-{version}.js")
          .Include("~/scripts/knockout-{version}.debug.js")
          .Include("~/scripts/sammy-{version}.js")
          .Include("~/scripts/toastr.js")
          .Include("~/scripts/Q.js")
          .Include("~/scripts/breeze.debug.js")
          .Include("~/scripts/bootstrap.js")
          .Include("~/scripts/moment.js")
          .Include("~/scripts/underscore.js")
        );

      bundles.Add(
        new StyleBundle("~/Content/css")
          .Include("~/Content/ie10mobile.css")
          .Include("~/Content/bootstrap.css")
          .Include("~/Content/bootstrap-responsive.css")
          .Include("~/Content/font-awesome.min.css")
          .Include("~/Content/durandal.css")
          .Include("~/Content/toastr.css")
          .Include("~/Content/app.css")
        );
    }

    public static void AddDefaultIgnorePatterns(IgnoreList ignoreList)
    {
      if (ignoreList == null)
      {
        throw new ArgumentNullException("ignoreList");
      }

      ignoreList.Ignore("*.intellisense.js");
      ignoreList.Ignore("*-vsdoc.js");

      //ignoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
      //ignoreList.Ignore("*.min.js", OptimizationMode.WhenDisabled);
      //ignoreList.Ignore("*.min.css", OptimizationMode.WhenDisabled);
    }
  }
}