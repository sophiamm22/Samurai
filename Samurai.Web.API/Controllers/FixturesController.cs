﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

using Samurai.Web.API.Infrastructure;
using Samurai.Web.API.Hubs;
using Samurai.Web.API.Messaging.FootballSchedule;
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

    [HttpPut]
    public async Task FetchTennisSchedules(TennisScheduleDateArgs commandArgs)
    {
      var command = new RequestWrapper<TennisScheduleDateArgs>(Request, commandArgs);
      await this.bus
                .SendWithSignalRCallback<TennisScheduleDateArgs, OddsHub>(command);
    }

    [HttpGet]
    [ActionName("todays-tennis-schedule")]
    public async Task<HttpResponseMessage> GetTodaysTennisSchedule()
    {
      TennisScheduleDateArgs requestArgs = null;
      var request = new RequestWrapper<TennisScheduleDateArgs>(Request, requestArgs);
      return await this.bus
                       .RequestReply(request);
    }

    [HttpGet]
    [ActionName("tennis-schedule-from")]
    public async Task<HttpResponseMessage> GetTodaysTennisSchedulesFromDate([FromUri]TennisScheduleDateArgs requestArgs)
    {
      var request = new RequestWrapper<TennisScheduleDateArgs>(Request, requestArgs);
      return await this.bus
                       .RequestReply(request);
    }

    [HttpGet]
    [ActionName("todays-football-schedule")]
    public async Task<HttpResponseMessage> GetTodaysFootballSchedule()
    {
      FootballScheduleDateArgs requestArgs = null;
      var request = new RequestWrapper<FootballScheduleDateArgs>(Request, requestArgs);
      return await this.bus
                       .RequestReply(request);
    }

    [HttpGet]
    [ActionName("football-schedule-from")]
    public async Task<HttpResponseMessage> GetTodaysFootballSchedulesFromDate([FromUri]FootballScheduleDateArgs requestArgs)
    {
      var request = new RequestWrapper<FootballScheduleDateArgs>(Request, requestArgs);
      return await this.bus
                       .RequestReply(request);
    }

  }
}
