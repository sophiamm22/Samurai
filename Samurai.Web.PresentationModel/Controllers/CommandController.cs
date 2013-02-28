using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc; 
using System.Linq.Expressions;

using Samurai.WebPresentationModel.Messaging;
using Samurai.WebPresentationModel.MVCHelpers;
using Samurai.WebPresentationModel.ActionResults;

namespace Samurai.WebPresentationModel.Controllers
{
  public abstract class CommandController : Controller
  {
    protected IBus bus;

    public CommandController(IBus bus)
    {
      this.bus = bus;
    }

    public RedirectToRouteResult RedirectToAction<TController>(Expression<Func<TController, object>> actionExpression)
    {
      var controllerName = typeof(TController).GetControllerName();
      var actionName = actionExpression.GetActionName();

      return RedirectToAction(actionName, controllerName);
    }

    public RedirectToRouteResult RedirectToAction<TController>(Expression<Func<TController, object>> actionExpression, IDictionary<string, object> dictionary)
    {
      var controllerName = typeof(TController).GetControllerName();
      var actionName = actionExpression.GetActionName();

      return RedirectToAction(actionName, controllerName, new RouteValueDictionary(dictionary));
    }

    public RedirectToRouteResult RedirectToAction<TController>(Expression<Func<TController, object>> actionExpression, object values)
    {
      var controllerName = typeof(TController).GetControllerName();
      var actionName = actionExpression.GetActionName();

      return RedirectToAction(actionName, controllerName, new RouteValueDictionary(values));
    }

    public MessageActionResult<TRequest, TReply> Message<TRequest, TReply>(TRequest request,
      Func<TReply, ActionResult> success, Func<TRequest, ActionResult> failure)
      where TRequest : IRequest, new()
      where TReply : IReply, new()
    {
      return new MessageActionResult<TRequest, TReply>(this.bus, request, success, failure);
    }

    public MessageActionResult<TRequest, TReply> Message<TRequest, TReply>(TRequest request,
      Func<ActionResult> success, Func<ActionResult> failure)
      where TRequest : IRequest, new()
      where TReply : IReply, new()
    {
      return new MessageActionResult<TRequest, TReply>(this.bus, request, m => success(), m => failure());
    }

    public UnRoutedMessageActionResult<TRequest, TReply> Message<TRequest, TReply>(TRequest request,
      Func<TReply, ActionResult> allRoutes)
      where TRequest : IRequest, new()
      where TReply : IReply, new()
    {
      return new UnRoutedMessageActionResult<TRequest, TReply>(this.bus, request, allRoutes);
    }

    public UnRoutedMessageActionResult<TRequest, TReply> Message<TRequest, TReply>(TRequest request,
      Func<ActionResult> allRoutes)
      where TRequest : IRequest, new()
      where TReply : IReply, new()
    {
      return new UnRoutedMessageActionResult<TRequest, TReply>(this.bus, request, m => allRoutes());
    }


  }
}
