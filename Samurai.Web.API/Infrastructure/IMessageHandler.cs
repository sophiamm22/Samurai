using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNet.SignalR.Hubs;

namespace Samurai.Web.API.Infrastructure
{
  public interface IMessageHandler<TRequest>
    where TRequest : class
  {
    Task<HttpResponseMessage> Handle(RequestWrapper<TRequest> requestWrapper);
  }

  public interface IMessageHandlerWithSignalRHub<TRequest, THub>
    where TRequest : class
    where THub : IHub
  {
    Task<HttpResponseMessage> Handle(RequestWrapper<TRequest> requestWrapper);
  }
}
