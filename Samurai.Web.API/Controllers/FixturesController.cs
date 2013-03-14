using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Samurai.Web.API.Infrastructure;
using Samurai.Web.API.Messaging.TennisSchedule;

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

    [ActionName("gettennisschedule")]
    public HttpResponseMessage GetTennisSchedule(GetTennisScheduleArgs requestArgs)
    {
      var request = new RequestWrapper<GetTennisScheduleArgs>(Request, requestArgs);
      return this.bus.RequestReply(request);
    }

  }
}
