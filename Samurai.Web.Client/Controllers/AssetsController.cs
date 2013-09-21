using System.Web.Mvc;

namespace Samurai.Web.Client.Controllers
{
  public class AssetsController : Controller
  {
    public ActionResult ApiAuth()
    {
      var token = Session[Constants.SessionTokenKey] as string;
      var script = @"var my = my || {}; my.authToken = '" + token + "'; my.baseUri = '" + Constants.ApiBaseUri + "';";
      return JavaScript(script);
    }
  }
}
