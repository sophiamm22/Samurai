using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;

namespace Samurai.Web.Messaging.Infrastructure
{
  public interface IRequest
  {
    HttpRequestMessage RequestMessage { get; set; }
  }

  public abstract class Request : IRequest
  {
    public HttpRequestMessage RequestMessage { get; set; }

    public Request(HttpRequestMessage request)
    {
      if (request == null) throw new ArgumentNullException("request");
      RequestMessage = request;
    }
  }

}
