using System;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Web;
using System.Web.Http.Hosting;

using Samurai.Web.ViewModels.API;

namespace Samurai.Web.API.Infrastructure
{
  //shamelessly taken from JabbR
  public static class HttpRequestExtensions
  {
    public static HttpResponseMessage CreateSuccessMessage<T>(this HttpRequestMessage request, HttpStatusCode statusCode, T data, string filenamePrefix)
    {
      var responseMessage = request.CreateResponse(statusCode, data);
      return AddResponseHeaders(request, responseMessage, filenamePrefix);
    }
    public static HttpResponseMessage CreateSuccessMessage<T>(this HttpRequestMessage request, HttpStatusCode statusCode, T data)
    {
      var responseMessage = request.CreateResponse(statusCode, data);
      return AddResponseHeaders(request, responseMessage, null);
    }

    public static HttpResponseMessage CreateSuccessMessage(this HttpRequestMessage request, HttpStatusCode statusCode)
    {
      var responseMessage = request.CreateResponse(statusCode);
      return AddResponseHeaders(request, responseMessage, null);
    }

    public static HttpResponseMessage CreateErrorMessage(this HttpRequestMessage request, HttpStatusCode statusCode, string message, string filenamePrefix)
    {
      var responseMessage = request.CreateResponse(
          statusCode,
          new ErrorModel { Message = message },
          new MediaTypeHeaderValue("application/json"));

      return AddResponseHeaders(request, responseMessage, filenamePrefix);
    }
    public static HttpResponseMessage CreateErrorMessage(this HttpRequestMessage request, HttpStatusCode statusCode, string message)
    {
      var responseMessage = request.CreateResponse(
          statusCode,
          new ErrorModel { Message = message },
          new MediaTypeHeaderValue("application/json"));

      return AddResponseHeaders(request, responseMessage, null);
    }

    private static HttpResponseMessage AddResponseHeaders(HttpRequestMessage request, HttpResponseMessage responseMessage, string filenamePrefix)
    {
      return AddDownloadHeader(request, responseMessage, filenamePrefix);
    }
    private static HttpResponseMessage AddDownloadHeader(HttpRequestMessage request, HttpResponseMessage responseMessage, string filenamePrefix)
    {
      var queryString = new QueryStringCollection(request.RequestUri);
      bool download;
      if (queryString.TryGetAndConvert<bool>("download", out download))
      {
        if (download)
        {
          responseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = filenamePrefix + ".json" };
        }
      }
      else
      {
        return request.CreateResponse(
            HttpStatusCode.BadRequest,
            new ErrorModel { Message = "Value for download was specified but cannot be converted to true or false." },
            new MediaTypeHeaderValue("application/json"));
      }

      return responseMessage;
    }


  }
}
