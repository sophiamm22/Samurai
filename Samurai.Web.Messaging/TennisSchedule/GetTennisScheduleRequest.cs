using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;

using Samurai.Web.Messaging.Infrastructure;

namespace Samurai.Web.Messaging.TennisSchedule
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
