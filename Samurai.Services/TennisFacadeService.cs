using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Tennis;

namespace Samurai.Services
{
  public class TennisFacadeService : ITennisFacadeService
  {
    protected readonly ITennisFixtureService tennisFixtureService;
    protected readonly ITennisPredictionService tennisPredictionService;
    protected readonly ITennisOddsService tennisOddsService;

    public TennisFacadeService(ITennisFixtureService tennisFixtureService,
      ITennisPredictionService tennisPredictionService, ITennisOddsService tennisOddsService)
    {
      if (tennisFixtureService == null) throw new ArgumentNullException("tennisFixtureService");
      if (tennisPredictionService == null) throw new ArgumentNullException("tennisPredictionService");
      if (tennisOddsService == null) throw new ArgumentNullException("tennisOddsService");

      this.tennisFixtureService = tennisFixtureService;
      this.tennisPredictionService = tennisPredictionService;
      this.tennisOddsService = tennisOddsService;
    }

    
  }
}
