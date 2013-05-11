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


  }
}
