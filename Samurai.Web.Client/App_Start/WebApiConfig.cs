using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace Samurai.Web.Client.App_Start
{
  public class WebApiConfig
  {
    public static string ControllerActionDate = "ApiControllerActionAndDate";

    public static void Register(HttpConfiguration config)
    {
      config.Routes.MapHttpRoute(
        name: ControllerActionDate,
        routeTemplate: "api/{controller}/{action}/{day}/{month}/{year}",
        defaults: null,
        constraints: new
        {
          day = @"^\d{2}$",
          month = @"^\d{2}$",
          year = @"^\d{4}$"
        });

      // To disable tracing in your application, please comment out or remove the following line of code
      // For more information, refer to: http://www.asp.net/web-api
      //config.EnableSystemDiagnosticsTracing();

      var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
      config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
      // Use camel case for JSON data.
      //config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

    }
  }
}