using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Model;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.Domain.Value
{
  public interface IFixtureStrategyProvider
  {
    IFixtureStrategy CreateFixtureStrategy(SportEnum sport);
  }
  public class FixtureStrategyProvider : IFixtureStrategyProvider
  {
    protected readonly IFixtureRepository fixtureService;
    protected readonly IWebRepository webRepository;

    public FixtureStrategyProvider(IFixtureRepository fixtureService, IWebRepository webRepository)
    {
      this.fixtureService = fixtureService;
      this.webRepository = webRepository;
    }

    public IFixtureStrategy CreateFixtureStrategy(SportEnum sport)
    {
      if (sport == SportEnum.Football)
        return new FootballFixtureStrategy(this.fixtureService, this.webRepository);
      else if (sport == SportEnum.Tennis)
        return new TennisFixtureStrategy(this.fixtureService, this.webRepository);
      else
        throw new ArgumentException("Sport not recognised");
    }
    
  }
}
