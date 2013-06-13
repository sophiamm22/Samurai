using System;
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
  public class OddsController : ApiController
  {
    protected readonly IBus bus;

    public OddsController(IBus bus)
    {
      if (bus == null) throw new ArgumentNullException("bus");
      this.bus = bus;
    }

    [HttpGet]
    [ActionName("todays-tennis-odds")]
    public async Task<HttpResponseMessage> GetTodaysTennisOdds()
    {
      TennisOddsDateArgs requestArgs = null; // new TennisOddsDateArgs() { Day = 26, Month = 02, Year = 2013 }; 
      var request = new RequestWrapper<TennisOddsDateArgs>(Request, requestArgs);
      return await this.bus
                       .RequestReply(request);
    }

    [HttpGet]
    [ActionName("tennis-odds-from")]
    public async Task<HttpResponseMessage> GetTodaysTennisOddsFromDate([FromUri]TennisOddsDateArgs requestArgs)
    {
      var request = new RequestWrapper<TennisOddsDateArgs>(Request, requestArgs);
      return await this.bus
                       .RequestReply(request);
    }

    [HttpGet]
    [ActionName("todays-football-odds")]
    public async Task<HttpResponseMessage> GetTodaysFootballOdds()
    {
      FootballOddsDateArgs requestArgs = null; // new FootballOddsDateArgs() { Day = 26, Month = 02, Year = 2013 };
      var request = new RequestWrapper<FootballOddsDateArgs>(Request, requestArgs);
      return await this.bus
                       .RequestReply(request);
    }

    [HttpGet]
    [ActionName("football-odds-from")]
    public async Task<HttpResponseMessage> GetTodaysFootballOddsFromDate([FromUri]FootballOddsDateArgs requestArgs)
    {
      var request = new RequestWrapper<FootballOddsDateArgs>(Request, requestArgs);
      return await this.bus
                       .RequestReply(request);
    }
  }
}
