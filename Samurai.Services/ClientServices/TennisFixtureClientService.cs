using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels.Tennis;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Model;
using Samurai.Services.AdminServices;
using Samurai.Domain.Infrastructure;

namespace Samurai.Services.ClientServices
{
  public class TennisFixtureClientService : ITennisFixtureClientService
  {
    private readonly ITennisFacadeAdminService tennisAdminService;

    public TennisFixtureClientService(ITennisFacadeAdminService tennisAdminService) 
    {
      if (tennisAdminService == null) throw new ArgumentNullException("tennisAdminService");
      this.tennisAdminService = tennisAdminService;
    }

    public IEnumerable<TennisFixtureViewModel> GetDaysSchedule(DateTime fixtureDate)
    {
      return this.tennisAdminService.GetDaysSchedule(fixtureDate);
    }
  }
}
