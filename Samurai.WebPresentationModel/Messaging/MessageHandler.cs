using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.WebPresentationModel.Messaging
{
  public abstract class MessageHandler<TRequest, TReply> : IMessageHandler<TRequest, TReply>
    where TRequest : IRequest, new()
    where TReply : IReply, new()
  {
    protected TReply reply;

    public MessageHandler()
    {
      reply = new TReply { Success = false, ModelErrors = new Dictionary<string, string>() };
    }

    public abstract TReply Handle(TRequest request);

  }
}
