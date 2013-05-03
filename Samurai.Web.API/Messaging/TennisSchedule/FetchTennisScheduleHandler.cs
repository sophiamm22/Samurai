using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

using Samurai.Web.API.Infrastructure;
using Samurai.Services.Contracts;
using Samurai.Web.ViewModels.Tennis;
using Samurai.Web.API.Hubs;

namespace Samurai.Web.API.Messaging.TennisSchedule
{
  public class FetchTennisScheduleHandler : CommandHandlerWithSignalRHub<TennisScheduleDateArgs, OddsHub>
  {
    private readonly ITennisFacadeAdminService tennisService;

    public FetchTennisScheduleHandler(ITennisFacadeAdminService tennisService)
      : base()
    {
      if (tennisService == null) throw new ArgumentNullException("tennisService");
      this.tennisService = tennisService;
    }

    public override async Task Handle(RequestWrapper<TennisScheduleDateArgs> commandWrapper)
    {
      throw new NotImplementedException();
    }
  }
}