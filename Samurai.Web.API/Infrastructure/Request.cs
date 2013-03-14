using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;

namespace Samurai.Web.API.Infrastructure
{
  public interface IRequestWrapper<T>
    where T : class
  {
    HttpRequestMessage RequestMessage { get; set; }
    T RequestArguments { get; set; }
  }

  public class RequestWrapper<T> : IRequestWrapper<T>
    where T : class
  {
    public HttpRequestMessage RequestMessage { get; set; }
    public T RequestArguments { get; set; }

    public RequestWrapper(HttpRequestMessage request, T args)
    {
      if (request == null) throw new ArgumentNullException("request");
      if (args == null) throw new ArgumentNullException("args");
      RequestMessage = request;
      RequestArguments = args;
    }

  }

}
