using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.WebPresentationModel.Messaging
{
  public interface IMessageHandler<TRequest, TReply>
    where TRequest : IRequest, new()
    where TReply : IReply, new()
  {
    TReply Handle(TRequest request);
  }
}
