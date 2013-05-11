using System;
using System.Web.Mvc;
using Samurai.WebPresentationModel.Messaging;

namespace Samurai.WebPresentationModel.ActionResults
{
  public class CommandActionResult<TCommand, TFeature> : ActionResult
    where TCommand : class, IRequest
  {
    private readonly IBus bus;
    private readonly TCommand command;
    private readonly Func<TCommand, ActionResult> redirect;

    public CommandActionResult(IBus bus, TCommand command,
      Func<TCommand, ActionResult> redirect)
    {
      this.bus = bus;
      this.command = command;
      this.redirect = redirect;
    }

    public TCommand Command { get { return this.command; } }

    public ActionResult Redirect { get { return this.redirect(command); } }

    public override void ExecuteResult(ControllerContext context)
    {
      Execute(context);
    }

    public void Execute(ControllerContext context)
    {
      this.bus.Send<TCommand>(command);
    }

  }
}
