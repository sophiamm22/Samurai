using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Samurai.Web.API.Infrastructure;
using Samurai.Services.Contracts;
using Samurai.Web.ViewModels.Tennis;
using Samurai.Web.API.Hubs;

namespace Samurai.Web.API.Messaging.TennisSchedule
{
  public class FetchTennisScheduleHandler : CommandHandlerWithSignalRHub<TennisScheduleArgs, OddsHub>
  {
    private readonly ITennisFacadeAdminService tennisService;

    public FetchTennisScheduleHandler(ITennisFacadeAdminService tennisService)
      : base()
    {
      if (tennisService == null) throw new ArgumentNullException("tennisService");
      this.tennisService = tennisService;
    }

    public override void Handle(RequestWrapper<TennisScheduleArgs> commandWrapper)
    {
      throw new NotImplementedException();
    }
  }
}