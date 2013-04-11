using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Samurai.Web.API.Infrastructure
{
  public abstract class CommandHandlerWithSignalRHub<TCommand, THub> : ICommandHandlerWithSignalRHub<TCommand, THub>
    where TCommand : class
    where THub : IHub
  {
    private Lazy<IHubContext> hub = new Lazy<IHubContext>(
      () => GlobalHost.ConnectionManager.GetHubContext<THub>()
     );

    protected IHubContext Hub { get { return this.hub.Value; } }

    public abstract void Handle(RequestWrapper<TCommand> commandWrapper);
  }
}