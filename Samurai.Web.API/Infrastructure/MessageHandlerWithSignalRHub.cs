using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Samurai.Web.API.Infrastructure
{
  //similar to https://github.com/bradwilson/ndc2012/blob/master/NdcDemo/Controllers/ApiControllerWithHub.cs
  public abstract class MessageHandlerWithSignalRHub<TRequest, THub> : IMessageHandlerWithSignalRHub<TRequest, THub>
    where TRequest : class
    where THub : IHub
  {
    private Lazy<IHubContext> hub = new Lazy<IHubContext>(
      () => GlobalHost.ConnectionManager.GetHubContext<THub>()
    );

    protected IHubContext Hub { get { return this.hub.Value; } }

    public abstract HttpResponseMessage Handle(RequestWrapper<TRequest> requestWrapper);
  }
}