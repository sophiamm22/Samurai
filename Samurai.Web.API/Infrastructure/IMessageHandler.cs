using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;

namespace Samurai.Web.API.Infrastructure
{
  public interface IMessageHandler<TRequest>
    where TRequest : class, IRequest
  {
    HttpResponseMessage Handle(TRequest request);
  }
}
