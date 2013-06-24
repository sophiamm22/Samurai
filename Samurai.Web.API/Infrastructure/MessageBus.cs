using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Samurai.Web.API.Infrastructure
{
  // liked it: http://trycatchfail.com/blog/post/RageFeede28099s-Message-Bus.aspx
  // uses a service locator though, so swapped in the TypedFactoryFacility
  public interface IBus
  {
    Task SendSignal<TCommand>(TCommand command)
      where TCommand : class;

    Task Send<TCommand>(RequestWrapper<TCommand> command)
      where TCommand : class;

    Task SendWithSignalRCallback<TCommand, THub>(RequestWrapper<TCommand> command)
      where TCommand : class
      where THub : IHub;

    Task<HttpResponseMessage> RequestReply<TRequest>(RequestWrapper<TRequest> request)
      where TRequest : class;

    Task<HttpResponseMessage> RequestReplyWithSignalRCallback<TRequest, THub>(RequestWrapper<TRequest> request)
      where TRequest : class
      where THub : IHub;
  }

  public class MessageBus : IBus
  {
    private readonly ICommandHandlerFactory commandHandlerFactory;
    private readonly IMessageHandlerFactory messageHandlerFactory;
    private readonly ISignalHandlerFactory signalHandlerFactory;

    public MessageBus(ICommandHandlerFactory commandHandlerFactory,
      IMessageHandlerFactory messageHandlerFactory, ISignalHandlerFactory signalHandlerFactory)
    {
      this.messageHandlerFactory = messageHandlerFactory;
      this.commandHandlerFactory = commandHandlerFactory;
      this.signalHandlerFactory = signalHandlerFactory;
    }

    public async Task SendSignal<TCommand>(TCommand command)
      where TCommand : class
    {
      var handler = this.signalHandlerFactory.Create<ISignalHandler<TCommand>>();
      if (handler == null)
        throw new ArgumentNullException(string.Format("signal<{0}>", typeof(TCommand).Name));

      await handler.Handle(command);
      this.signalHandlerFactory.Release(handler);
    }


    public async Task Send<TCommand>(RequestWrapper<TCommand> message)
      where TCommand : class
    {
      var handler = this.commandHandlerFactory.Create<ICommandHandler<TCommand>>();
      if (handler == null)
        throw new ArgumentNullException(string.Format("commandHandler<{0}>", typeof(TCommand).Name));

      await handler.Handle(message);
      this.commandHandlerFactory.Release(handler);
    }

    public async Task SendWithSignalRCallback<TCommand, THub>(RequestWrapper<TCommand> message)
      where TCommand : class
      where THub : IHub
    {
      var handler = this.commandHandlerFactory.Create<ICommandHandlerWithSignalRHub<TCommand, THub>>();
      if (handler == null)
        throw new ArgumentNullException(string.Format("commandHandler<{0}, {1}>", typeof(TCommand).Name, typeof(THub).Name));

      await handler.Handle(message);
      this.commandHandlerFactory.Release(handler);
    }

    public async Task<HttpResponseMessage> RequestReply<TRequest>(RequestWrapper<TRequest> request) 
      where TRequest : class
    {
      var handler = this.messageHandlerFactory.Create<IMessageHandler<TRequest>>();
      if (handler == null)
        throw new ArgumentNullException(string.Format("messageHandler<{0}>", typeof(TRequest).Name));

      var reply = await handler.Handle(request);
      this.messageHandlerFactory.Release(handler);

      return reply;
    }

    public async Task<HttpResponseMessage> RequestReplyWithSignalRCallback<TRequest, THub>(RequestWrapper<TRequest> request)
      where TRequest : class
      where THub : IHub
    {
      var handler = this.messageHandlerFactory.Create<IMessageHandlerWithSignalRHub<TRequest, THub>>();
      if (handler == null)
        throw new ArgumentNullException(string.Format("MessageHandlerWithSignalRHub<{0}, {1}>", typeof(TRequest).Name, typeof(THub).Name));

      var reply = await handler.Handle(request);
      this.messageHandlerFactory.Release(handler);

      return reply;
    }

  }
}
