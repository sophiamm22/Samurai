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
    protected readonly IPredictionRepository predictionRepository;
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly IWebRepository webRepository;

    public PredictionProvider(IPredictionRepository predictionRepository, IWebRepository webRepository)
    {
      this.predictionRepository = predictionRepository;
      this.webRepository = webRepository;
    }

    public AbstractPredictionStrategy CreatePredictionStrategy(IValueOptions valueOptions, bool overrideExisting)
    {
      if (valueOptions.Sport.SportName == "Football")
        return new FootballFinkTankPredictionStrategy(this.predictionRepository, this.fixtureRepository, this.webRepository);
      else if (valueOptions.Sport.SportName == "Tennis")
        return new TennisPredictionStrategy(this.predictionRepository, this.fixtureRepository, this.webRepository);
      else
        throw new ArgumentException("Sport not recognised");
    }
  }
}
