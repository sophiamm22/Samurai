using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Domain.Model;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.Domain.Value
{
  public interface IPredictionProvider
  {
    AbstractPredictionStrategy CreatePredictionStrategy(IValueOptions valueOptions, bool overrideExisting);
  }

  public class PredictionProvider : IPredictionProvider
  {
    protected readonly IPredictionRepository predictionService;
    protected readonly IWebRepository webRepository;

    public PredictionProvider(IPredictionRepository predictionService, IWebRepository webRepository)
    {
      this.predictionService = predictionService;
      this.webRepository = webRepository;
    }

    public AbstractPredictionStrategy CreatePredictionStrategy(IValueOptions valueOptions, bool overrideExisting)
    {
      if (valueOptions.Sport.SportName == "Football")
        return new FootballFinkTankPredictionStrategy(this.predictionService, this.webRepository);
      else if (valueOptions.Sport.SportName == "Tennis")
        return new TennisPredictionStrategy(this.predictionService, this.webRepository);
      else
        throw new ArgumentException("Sport not recognised");
    }
  }
}
