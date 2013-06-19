using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Samurai.Web.API.Infrastructure;
using Samurai.Services.Contracts.Async;
using Samurai.Web.ViewModels.Tennis;
using Samurai.Web.API.Hubs;

namespace Samurai.Web.API.Messaging.TennisSchedule
{
  public class FetchTennisScheduleHandler : CommandHandlerWithSignalRHub<TennisScheduleDateArgs, OddsHub>
  {
    private readonly IAsyncTennisFacadeAdminService tennisService;

    public FetchTennisScheduleHandler(IAsyncTennisFacadeAdminService tennisService)
      : base()
    {
      if (tennisService == null) throw new ArgumentNullException("tennisService");
      this.tennisService = tennisService;
    }

    public override async Task Handle(RequestWrapper<TennisScheduleDateArgs> requestWrapper)
    {
      if (!Extensions.IsValidDate(requestWrapper.RequestArguments.Year, requestWrapper.RequestArguments.Month, requestWrapper.RequestArguments.Day))
      {
        Hub.Clients.All.reportProgress("Not a valid date");
      }

      try
      {
        var fixtureDate = new DateTime(requestWrapper.RequestArguments.Year, requestWrapper.RequestArguments.Month, requestWrapper.RequestArguments.Day);
        var tennisFixtures = await this.tennisService.UpdateDaysSchedule(fixtureDate);
        Hub.Clients.All.reportProgress("done");
      }
      catch (Exception ex)
      {
        Hub.Clients.All.reportProgress("Oops " + ex.Message);
      }
    }
  }
}