using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Samurai.Web.API.Infrastructure;
using Samurai.Services.Contracts.Async;
using Samurai.Web.ViewModels.Football;

namespace Samurai.Web.API.Messaging.FootballSchedule
{
  public class GetFootballOddsHandler : IMessageHandler<FootballOddsDateArgs>
  {
    private readonly IAsyncFootballFacadeClientService footballService;

    public GetFootballOddsHandler(IAsyncFootballFacadeClientService footballService)
    {
      if (footballService == null) throw new ArgumentNullException("footballService");
      this.footballService = footballService;
    }

    public async Task<HttpResponseMessage> Handle(RequestWrapper<FootballOddsDateArgs> requestWrapper)
    {
      DateTime fixtureDate;
      if (requestWrapper.RequestArguments == null)
      {
        fixtureDate =
          this.footballService
              .GetLatestDate();
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

      IQueryable<FootballCouponViewModel> footballCoupons;
      try
      {
        footballCoupons = (await
          this.footballService
              .GetDaysOdds(fixtureDate))
              .AsQueryable();
      }
      catch (Exception ex)
      {
        return requestWrapper.RequestMessage
                             .CreateErrorMessage(HttpStatusCode.NotFound, ex.Message);
      }
      return requestWrapper.RequestMessage
                           .CreateSuccessMessage(HttpStatusCode.OK, footballCoupons);
    }

  }
}