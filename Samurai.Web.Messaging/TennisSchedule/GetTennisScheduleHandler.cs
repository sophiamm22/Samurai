using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;

using Samurai.Web.Messaging.Infrastructure;
using Samurai.Web.Messaging.TennisSchedule;
using Samurai.Services.Contracts;
using Samurai.Web.ViewModels.Tennis;

namespace Samurai.Web.Messaging.TennisSchedule
{
  public class GetTennisScheduleHandler : IMessageHandler<GetTennisScheduleRequest>
  {
    private readonly ITennisFacadeService tennisService;

    public GetTennisScheduleHandler(ITennisFacadeService tennisService)
      :base()
    {
      if (tennisService == null) throw new ArgumentNullException("tennisService");
      this.tennisService = tennisService;
    }

    public HttpResponseMessage Handle(GetTennisScheduleRequest request)
    {
      if (!Extensions.IsValidDate(request.Year, request.Month, request.Day))
        return request.RequestMessage.CreateErrorMessage(HttpStatusCode.BadRequest, "not a valid date");

      var fixtureDate = new DateTime(request.Year, request.Month, request.Day);

      IEnumerable<TennisFixtureViewModel> tennisFixtures;
      try
      {
        tennisFixtures = this.tennisService.GetDaysSchedule(fixtureDate);
      }
      catch (Exception ex)
      {
        return request.RequestMessage.CreateErrorMessage(HttpStatusCode.NotFound, ex.Message);
      }

      return request.RequestMessage.CreateSuccessMessage(HttpStatusCode.OK, tennisFixtures);
    }

  }
}
