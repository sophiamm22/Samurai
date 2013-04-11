using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Samurai.Web.API.Infrastructure;
using Samurai.Web.API.Messaging.TennisSchedule;
using Samurai.Web.API.Hubs;

using Breeze.WebApi;

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

    [HttpPut]
    public void FetchTennisSchedules(TennisScheduleArgs commandArgs)
    {
      var command = new RequestWrapper<TennisScheduleArgs>(Request, commandArgs);
      this.bus.SendWithSignalRCallback<TennisScheduleArgs, OddsHub>(command);
    }

    [HttpGet]
    [ActionName("TennisSchedules")]
    public HttpResponseMessage GetTennisSchedules(TennisScheduleArgs requestArgs)
    {
      var request = new RequestWrapper<TennisScheduleArgs>(Request, requestArgs);
      return this.bus.RequestReply(request);
    }

  }
}
