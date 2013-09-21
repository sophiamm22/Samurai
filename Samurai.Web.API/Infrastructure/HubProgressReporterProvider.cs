using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

using Samurai.Domain.Model;
using Samurai.Domain.Infrastructure;
using Samurai.Web.API.Hubs;

namespace Samurai.Web.API.Infrastructure
{
  public class HubProgressReporterProvider : ProgressReporterProvider
  {
    private Lazy<IHubContext> hub;
    public HubProgressReporterProvider()
    {
      this.hub = new Lazy<IHubContext>(
        () => GlobalHost.ConnectionManager.GetHubContext<OddsHub>()
        ); 
    }
    public override void ReportProgress(string message, ReporterImportance importance, ReporterAudience audience)
    {
      this.hub.Value.Clients.All.reportProgress(message);
    }
  }
}