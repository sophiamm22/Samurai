using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Samurai.Web.Messaging.Infrastructure
{
  // liked it: http://trycatchfail.com/blog/post/RageFeede28099s-Message-Bus.aspx
  // uses a service locator though, so swapped in the TypedFactoryFacility
  public interface IBus
  {
    void Send<TMessage>(TMessage message);
    HttpResponseMessage RequestReply<TRequest>(TRequest request)
      where TRequest : class, IRequest;
  }

  public class MessageBus : IBus
  {
    private readonly IMessageHandlerFactory messageHandlerFactory;
    private readonly ICommandHandlerFactory commandHandlerFactory;

    public MessageBus(IMessageHandlerFactory messageHandlerFactory,
      ICommandHandlerFactory commandHandlerFactory)
    {
      this.messageHandlerFactory = messageHandlerFactory;
      this.commandHandlerFactory = commandHandlerFactory;
    }

    public void Send<TMessage>(TMessage message)
    {
      throw new NotImplementedException();
    }

    public HttpResponseMessage RequestReply<TRequest>(TRequest request) 
      where TRequest : class, IRequest
    {
      var handler = this.messageHandlerFactory.Create<IMessageHandler<TRequest>>();
      if (handler == null)
        throw new ArgumentNullException(string.Format("MessageHandler<{0}>",
          typeof(TRequest).Name));

      var reply = handler.Handle(request);
      this.messageHandlerFactory.Release(handler);

      return reply;
    }

  }
}
