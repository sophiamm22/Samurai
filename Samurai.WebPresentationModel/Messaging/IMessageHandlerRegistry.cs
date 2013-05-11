using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.WebPresentationModel.Messaging
{
  public interface IMessageHandlerRegistry
  {
    ICommandHandler<TMessage> GetCommandHandlerFor<TMessage>();   //need constraints when implemented

    IMessageHandler<TRequest, TReply> GetMessageHandlerFor<TRequest, TReply>() 
      where TRequest : class, IRequest
      where TReply : class, IReply;
  }

  public class MessageHandlerRegistry
  {
    private readonly IMessageHandlerFactory messageHandlerFactory;
    private readonly ICommandHandlerFactory commandHandlerFactory;

    public MessageHandlerRegistry(IMessageHandlerFactory messageHandlerFactory, 
      ICommandHandlerFactory commandHandlerFactory)
    {
      this.messageHandlerFactory = messageHandlerFactory;
      this.commandHandlerFactory = commandHandlerFactory;
    }

    public ICommandHandler<TMessage> GetCommandHandlerFor<TMessage>()
    {
      throw new NotImplementedException();
    }

    public IMessageHandler<TRequest, TReply> GetMessageHandlerFor<TRequest, TReply>()
      where TRequest : class, IRequest
      where TReply : class, IReply
    {
      return this.messageHandlerFactory.Create<IMessageHandler<TRequest, TReply>>();
    }

  }
}
