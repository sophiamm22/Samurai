using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

using Samurai.Web.API;
using Samurai.Web.API.Infrastructure;
using Samurai.Web.API.Messaging.TennisSchedule;
using Samurai.Services.Contracts.Async;

namespace Samurai.Web.API.Hubs
{
  public class OddsHub : Hub
  {
    private readonly IAsyncTennisFacadeAdminService tennisService;

    public OddsHub(IAsyncTennisFacadeAdminService tennisService)
      : base()
    {
      if (tennisService == null) throw new ArgumentNullException("tennisService");
      this.tennisService = tennisService;
    }

    public Task<string> FetchTennisSchedules(string dateString)
    {
      var dateParts = dateString.Split('-');
      int day, month, year;
      if (int.TryParse(dateParts[0], out year) && int.TryParse(dateParts[1], out month) && int.TryParse(dateParts[2], out day))
      {
        if (!Extensions.IsValidDate(year, month, day))
        {
          return Task.Run(() => "Not a valid date");
        }

        var fixtureDate = new DateTime(year, month, day);
        
        return Task.Run(async () =>
          {
            try
            {
              await this.tennisService.UpdateDaysSchedule(fixtureDate);
            }
            catch (Exception ex)
            {
              return ex.Message;
            }
            return "Started getting tennis schedule";
          });
      }
      return Task.Run(() => "Started getting tennis schedule");
    }

  }
}