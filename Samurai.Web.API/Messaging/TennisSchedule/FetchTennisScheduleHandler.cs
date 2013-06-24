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
  public class FetchTennisScheduleHandler : ISignalHandler<FetchTennisScheduleDateArgs>
  {
    private readonly IAsyncTennisFacadeAdminService tennisService;

    public FetchTennisScheduleHandler(IAsyncTennisFacadeAdminService tennisService)
    {
      if (tennisService == null) throw new ArgumentNullException("tennisService");
      this.tennisService = tennisService;
    }

    public async Task Handle(FetchTennisScheduleDateArgs command)
    {
      if (!Extensions.IsValidDate(command.Year, command.Month, command.Day))
      {
        throw new ArgumentException();
      }

      try
      {
        var fixtureDate = new DateTime(command.Year, command.Month, command.Day);
        var tennisFixtures = await this.tennisService.UpdateDaysSchedule(fixtureDate);
      }
      catch (Exception ex)
      {

      }
    }
  }
}