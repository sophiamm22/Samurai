using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Web.Messaging.Infrastructure
{
  public interface ICommandHandlerFactory
  {
    T Create<T>();
    void Release(object command);
  }
}
