using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Microsoft.AspNet.SignalR.Hubs;

namespace Samurai.Web.API.Infrastructure
{
  public interface IMessageHandler<TRequest>
    where TRequest : class
  {
    HttpResponseMessage Handle(RequestWrapper<TRequest> requestWrapper);
  }

  public interface IMessageHandlerWithSignalRHub<TRequest, THub>
    where TRequest : class
    where THub : IHub
  {
    HttpResponseMessage Handle(RequestWrapper<TRequest> requestWrapper);
  }
}
