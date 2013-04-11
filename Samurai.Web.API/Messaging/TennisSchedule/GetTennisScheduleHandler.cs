﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;

using Samurai.Web.API.Infrastructure;
using Samurai.Services.Contracts;
using Samurai.Web.ViewModels.Tennis;

namespace Samurai.Web.API.Messaging.TennisSchedule
{
  public class GetTennisScheduleHandler : IMessageHandler<TennisScheduleArgs>
  {
    private readonly ITennisFixtureClientService tennisService;

    public GetTennisScheduleHandler(ITennisFixtureClientService tennisService)
    {
      if (tennisService == null) throw new ArgumentNullException("tennisService");
      this.tennisService = tennisService;
    }

    public HttpResponseMessage Handle(RequestWrapper<TennisScheduleArgs> requestWrapper)
    {
      if (!Extensions.IsValidDate(requestWrapper.RequestArguments.Year, requestWrapper.RequestArguments.Month, requestWrapper.RequestArguments.Day))
        return requestWrapper.RequestMessage.CreateErrorMessage(HttpStatusCode.BadRequest, "not a valid date");

      var fixtureDate = new DateTime(requestWrapper.RequestArguments.Year, requestWrapper.RequestArguments.Month, requestWrapper.RequestArguments.Day);

      IQueryable<TennisFixtureViewModel> tennisFixtures;
      try
      {
        tennisFixtures = 
          this.tennisService
              .GetDaysSchedule(fixtureDate)
              .AsQueryable();
      }
      catch (Exception ex)
      {
        return requestWrapper.RequestMessage.CreateErrorMessage(HttpStatusCode.NotFound, ex.Message);
      }
      return requestWrapper.RequestMessage.CreateSuccessMessage(HttpStatusCode.OK, tennisFixtures);
    }

  }
}
