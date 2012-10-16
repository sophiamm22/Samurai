using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.WebPresentationModel.Messaging
{
  public interface IMessageHandlerFactory
  {
    T Create<T>();
    void Release(object handler);
  }
}
