using System;
using System.Web.Mvc;
using Samurai.WebPresentationModel.Messaging;

namespace Samurai.WebPresentationModel.ActionResults
{
  public class UnRoutedMessageActionResult<TRequest, TReply> : ActionResult
    where TRequest : IRequest, new()
    where TReply : IReply, new()
  {
    private readonly IBus bus;
    private readonly TRequest request;
    private readonly Func<TReply, ActionResult> allRoutes;
    private TReply reply;

    public UnRoutedMessageActionResult(IBus bus, TRequest request,
      Func<TReply, ActionResult> allRoutes)
    {
      this.bus = bus;
      this.request = request;
      this.allRoutes = allRoutes;
    }

    public TRequest Request { get { return this.request; } }

    public ActionResult AllRoutes { get { return this.allRoutes(reply); } }

    public override void ExecuteResult(ControllerContext context)
    {
      Execute(context);
    }

    public void Execute(ControllerContext context)
    {
      var modelState = context.Controller.ViewData.ModelState;

      if (modelState.IsValid)
      {
        var reply = this.bus.RequestReply<TRequest, TReply>(this.request);
        if (reply.Success)
        {
          this.reply = reply;
        }

        foreach (var error in reply.ModelErrors)
        {
          modelState.AddModelError(error.Key, error.Value);
        }
      }
      AllRoutes.ExecuteResult(context);
    }

  }
}

