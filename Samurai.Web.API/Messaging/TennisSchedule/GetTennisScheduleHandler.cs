using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Samurai.Web.API.Infrastructure;
using Samurai.Services.Contracts.Async;
using Samurai.Web.ViewModels.Tennis;

namespace Samurai.Web.API.Messaging.TennisSchedule
{
  public class GetTennisScheduleHandler : IMessageHandler<TennisScheduleDateArgs>
  {
    private readonly IAsyncTennisFacadeClientService tennisService;

    public GetTennisScheduleHandler(IAsyncTennisFacadeClientService tennisService)
    {
      if (tennisService == null) throw new ArgumentNullException("tennisService");
      this.tennisService = tennisService;
    }

    public async Task<HttpResponseMessage> Handle(RequestWrapper<TennisScheduleDateArgs> requestWrapper)
    {
      DateTime fixtureDate;
      if (requestWrapper.RequestArguments == null)
      {
        fixtureDate = this.tennisService.GetLatestDate();
      }
      else if (!Extensions.IsValidDate(requestWrapper.RequestArguments.Year, requestWrapper.RequestArguments.Month, requestWrapper.RequestArguments.Day))
      {
        return requestWrapper.RequestMessage.CreateErrorMessage(HttpStatusCode.BadRequest, "not a valid date");
      }
      else
      {
        fixtureDate = new DateTime(requestWrapper.RequestArguments.Year,
                                   requestWrapper.RequestArguments.Month,
                                   requestWrapper.RequestArguments.Day);
      }

      IQueryable<TennisFixtureViewModel> tennisFixtures;
      try
      {
        tennisFixtures = (await
          this.tennisService
              .GetDaysSchedule(fixtureDate))
              .AsQueryable();
      }
      catch (Exception ex)
      {
        return requestWrapper.RequestMessage
                             .CreateErrorMessage(HttpStatusCode.NotFound, ex.Message);
      }
      return requestWrapper.RequestMessage
                           .CreateSuccessMessage(HttpStatusCode.OK, tennisFixtures);
    }

  }
}
