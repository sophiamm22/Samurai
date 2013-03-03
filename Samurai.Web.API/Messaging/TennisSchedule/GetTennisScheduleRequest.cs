using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;

using Samurai.Web.API.Infrastructure;

namespace Samurai.Web.API.Messaging.TennisSchedule
{
  public class GetTennisScheduleRequest : Request 
  {
    public GetTennisScheduleRequest(HttpRequestMessage request)
      : base(request)
    { }

    public int Day { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }

  }
}
