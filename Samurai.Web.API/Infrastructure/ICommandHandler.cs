using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;

namespace Samurai.Web.API.Infrastructure
{
  public interface ICommandHandler<TCommand>
    where TCommand : class
  {
    void Handle(RequestWrapper<TCommand> commandWrapper);
  }

  public interface ICommandHandlerWithSignalRHub<TCommand, THub>
    where TCommand : class
    where THub : IHub
  {
    void Handle(RequestWrapper<TCommand> commandWrapper);
  }

}
