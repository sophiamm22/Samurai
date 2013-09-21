using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Samurai.Web.API.Infrastructure
{
  public interface ISignalHandlerFactory
  {
    T Create<T>();
    void Release(object signal);
  }
}