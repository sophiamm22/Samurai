using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Samurai.Web.Messaging.Infrastructure;
using Samurai.Web.Messaging.TennisSchedule;

namespace Samurai.Web.API.Controllers
{
  public class FixturesController : ApiController
  {
    protected readonly IBus bus;

    public FixturesController(IBus bus)
    {
      if (bus == null) throw new ArgumentNullException("bus");
      this.bus = bus;
    }

    public HttpResponseMessage GetTennisSchedule(int year, int month, int day)
    {
      var request = new GetTennisScheduleRequest(Request) { Year = year, Month = month, Day = day };
      return this.bus.RequestReply(request);
    }

  }
}
