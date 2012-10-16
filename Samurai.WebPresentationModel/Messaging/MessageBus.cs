using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.WebPresentationModel.Messaging
{
  // liked it: http://trycatchfail.com/blog/post/RageFeede28099s-Message-Bus.aspx
  // uses a service locator though, so swapped in the TypedFactoryFacility
  public interface IBus
  {
    void Send<TMessage>(TMessage message);
    TReply RequestReply<TRequest, TReply>(TRequest request) 
      where TRequest : IRequest, new()
      where TReply : IReply, new();
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
      //var handler = this.commandHandlerFactory.Create<TMessage>();
      //if (handler == null)
      //  throw new ArgumentNullException(string.Format("CommandHandler<{0}>", 
      //    typeof(TMessage).Name));

      //handler.Handle(message);
      throw new NotImplementedException();
    }

    public TReply RequestReply<TRequest, TReply>(TRequest request) 
      where TRequest : IRequest, new()
      where TReply : IReply, new() 
    {
      var handler = this.messageHandlerFactory.Create<IMessageHandler<TRequest, TReply>>();
      if (handler == null)
        throw new ArgumentNullException(string.Format("MessageHandler<{0},{1}>", 
          typeof(TRequest).Name, typeof(TReply).Name));

      var reply = handler.Handle(request);
      this.messageHandlerFactory.Release(handler);

      return reply;
    }

  }
}
