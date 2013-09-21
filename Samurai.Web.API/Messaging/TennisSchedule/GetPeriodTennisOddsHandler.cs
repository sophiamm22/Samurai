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
using Samurai.Web.ViewModels.Value;

namespace Samurai.Web.API.Messaging.TennisSchedule
{
  public class GetPeriodTennisOddsHandler : IMessageHandler<GetTennisOddsForPeriodAgs>
  {
    private readonly IAsyncTennisFacadeClientService tennisService;

    public GetPeriodTennisOddsHandler(IAsyncTennisFacadeClientService tennisService)
    {
      if (tennisService == null) throw new ArgumentNullException("tennisService");
      this.tennisService = tennisService;
    }

    public async Task<HttpResponseMessage> Handle(RequestWrapper<GetTennisOddsForPeriodAgs> requestWrapper)
    {
      DateTime startDate;
      DateTime endDate;
      if (requestWrapper.RequestArguments == null)
      {
        startDate = new DateTime(2010, 01, 01);
        endDate = new DateTime(2015, 12,31); //arbitrary-clearly not future proof
      }
      else if (!Extensions.IsValidDate(requestWrapper.RequestArguments.StartYear, requestWrapper.RequestArguments.StartMonth, requestWrapper.RequestArguments.StartDay))
      {
        return requestWrapper.RequestMessage.CreateErrorMessage(HttpStatusCode.BadRequest, "not a valid date");
      }
      else if (!Extensions.IsValidDate(requestWrapper.RequestArguments.EndYear, requestWrapper.RequestArguments.EndMonth, requestWrapper.RequestArguments.EndDay))
      {
        return requestWrapper.RequestMessage.CreateErrorMessage(HttpStatusCode.BadRequest, "not a valid date");
      }
      else
      {
        startDate 
          = new DateTime(requestWrapper.RequestArguments.StartYear,
                         requestWrapper.RequestArguments.StartMonth,
                         requestWrapper.RequestArguments.StartDay);
        endDate
          = new DateTime(requestWrapper.RequestArguments.EndYear,
                         requestWrapper.RequestArguments.EndMonth,
                         requestWrapper.RequestArguments.EndDay);
      }
      IQueryable<OddViewModel> historicTennisOdds;
      try
      {
        historicTennisOdds = (await
          this.tennisService
              .GetPeriodTennisOdds(startDate, endDate))
              .AsQueryable();
      }
      catch (Exception ex)
      {
        return requestWrapper.RequestMessage
                             .CreateErrorMessage(HttpStatusCode.NotFound, ex.Message);
      }
      return requestWrapper.RequestMessage
                           .CreateSuccessMessage(HttpStatusCode.OK, historicTennisOdds);
    }
    

  }
}