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
using Samurai.Domain.Exceptions;
using Samurai.Domain.Infrastructure;
using Samurai.Domain.Model;

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

    public Task FetchTennisSchedules(string dateString)
    {
      return Task.Run(async () =>
        {
          var dateParts = dateString.Split('-');
          int day, month, year;
          if (int.TryParse(dateParts[0], out year) && int.TryParse(dateParts[1], out month) && int.TryParse(dateParts[2], out day))
          {
            var fixtureDate = new DateTime(year, month, day);
            if (!Extensions.IsValidDate(year, month, day))
            {
              ProgressReporterProvider.Current.ReportProgress(string.Format("Not a valid date ({0}/{1}/{2})", day, month, year), ReporterImportance.Error, ReporterAudience.Admin);
              return;
            }
            try
            {
              await this.tennisService.UpdateDaysSchedule(fixtureDate);
            }
            catch (MissingTournamentCouponURLException mtcEx)
            {
              ProgressReporterProvider.Current.ReportProgress("Missing tournament coupon URLs..", ReporterImportance.Error, ReporterAudience.Admin);
              var missingTournamentCouponsURLs = mtcEx.MissingData;
              this.tennisService.RecordMissingTournamentCouponURLs(missingTournamentCouponsURLs);

              //update client to query the new missing records
            }
            catch (MissingTeamPlayerAliasException mtpaEx)
            {
              ProgressReporterProvider.Current.ReportProgress("Missing team or player alias..", ReporterImportance.Error, ReporterAudience.Admin);

              var missingTeamPlayerAliass = mtpaEx.MissingAlias;
              this.tennisService.RecordMissingTeamPlayerAlias(missingTeamPlayerAliass);

              //update client to query the new missing records
            }
            catch (Exception ex)
            {
              ProgressReporterProvider.Current.ReportProgress(string.Format("Exception thrown\n{0}", ex.Message), ReporterImportance.Error, ReporterAudience.Admin);
            }
          }
        });
    }
  }

}
