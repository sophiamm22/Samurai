using System;
using System.Web.Mvc;
using Samurai.WebPresentationModel.Messaging;

namespace Samurai.WebPresentationModel.ActionResults
{
  public class MessageActionResult<TRequest, TReply> : ActionResult 
    where TRequest : IRequest, new() 
    where TReply : IReply, new()
  {
    private readonly IBus bus;
    private readonly TRequest request;
    private readonly Func<TReply, ActionResult> success;
    private readonly Func<TRequest, ActionResult> failure;
    private TReply reply;

    public MessageActionResult(IBus bus, TRequest request, 
      Func<TReply, ActionResult> success, Func<TRequest, ActionResult> failure)
    {
      this.bus = bus;
      this.request = request;
      this.success = success;
      this.failure = failure;
    }

    public TRequest Request { get { return this.request; } }

    public ActionResult Success { get { return this.success(reply); } }

    public ActionResult Failure { get { return this.failure(request); } }

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
          Success.ExecuteResult(context);
          return;
        }

        foreach (var error in reply.ModelErrors)
        {
          modelState.AddModelError(error.Key, error.Value);
        }
      }
      Failure.ExecuteResult(context);
    }

  }
}
