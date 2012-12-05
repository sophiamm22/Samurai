using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Optimization;
using CoffeeSharp;

namespace Samurai.Web.App_Start
{
  public class CoffeeCompiler : IBundleTransform
  {
    public void Process(BundleContext context, BundleResponse response)
    {
      var coffeeEngine = new CoffeeScriptEngine();
      var compiledCoffeeScript = string.Empty;
      foreach (var file in response.Files)
      {
        using (var reader = new StreamReader(file.FullName))
        {
          compiledCoffeeScript += coffeeEngine.Compile(reader.ReadToEnd());
        }
      }

      response.Content = compiledCoffeeScript;
      response.ContentType = "text/javascript";
      response.Cacheability = HttpCacheability.Public;
    }
  }
}