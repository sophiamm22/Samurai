using System;
using System.Web.Optimization;

namespace Samurai.Web.Client
{
  public class BundleConfig
  {
    public static void RegisterBundles(BundleCollection bundles)
    {
      bundles.IgnoreList.Clear();
      bundles.UseCdn = true;

      AddDefaultIgnorePatterns(bundles.IgnoreList);

      // Modernizr
      bundles.Add(new ScriptBundle("~/bundles/modernizr")
        .Include("~/Scripts/modernizr-{version}.js"));

      // jQuery
      bundles.Add(new ScriptBundle("~/scripts/jquery",
          "//ajax.googleapis.com/ajax/libs/jquery/2.0.2/jquery.js")
          .Include("~/Scripts/jquery-{version}.js"));


      bundles.Add(
        new ScriptBundle("~/scripts/vendor")
          .Include("~/scripts/sammy-{version}.min.js")
          //.Include("~/scripts/jquery-{version}.js")
          .Include("~/scripts/knockout-{version}.debug.js")
          .Include("~/scripts/toastr.js")
          .Include("~/scripts/Q.js")
          .Include("~/scripts/breeze.debug.js")
          .Include("~/scripts/bootstrap.js")
          
          .Include("~/scripts/d3.v3.js")
          .Include("~/scripts/moment.js")
          .Include("~/scripts/underscore.js")
        );

      bundles.Add(
        new ScriptBundle("~/scripts/signalr")
          .Include("~/Scripts/jquery.signalR-{version}.js")
        );

      bundles.Add(
        new ScriptBundle("~/scripts/dc")//,
          //"//cdnjs.cloudflare.com/ajax/libs/dc/1.3.0/dc.min.js") ////cdnjs.cloudflare.com/ajax/libs/crossfilter/1.1.3/crossfilter.min.js
          .Include("~/Scripts/dc.js")
        );

      bundles.Add(
        new ScriptBundle("~/scripts/crossfilter")//,
          /* "//cdnjs.cloudflare.com/ajax/libs/crossfilter/1.1.3/crossfilter.min.js") */
          .Include("~/Scripts/crossfilter.js")
        );

      bundles.Add(
        new StyleBundle("~/Content/css")
          .Include("~/Content/ie10mobile.css")
          .Include("~/Content/bootstrap/bootstrap.css")
          //.Include("~/Content/bootstrap/bootstrap-theme.css")
          .Include("~/Content/font-awesome.min.css")
          .Include("~/Content/durandal.css")
          .Include("~/Content/toastr.css")
          .Include("~/Content/app.css")
          .Include("~/Content/dc.css")
        );

      BundleTable.EnableOptimizations = true;

      //bundles.Add(new Bundle("~/Content/Less", new LessTransform(), new CssMinify())
      //    .Include("~/Content/styles.less"));
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