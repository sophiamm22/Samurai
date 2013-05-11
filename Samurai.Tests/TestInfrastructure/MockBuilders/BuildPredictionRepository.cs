using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;

using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Model;
using E = Samurai.Domain.Entities;

namespace Samurai.Tests.TestInfrastructure.MockBuilders
{
  public static class BuildPredictionRepository
  {
    public static Mock<IPredictionRepository> Create()
    {
      return new Mock<IPredictionRepository>();
    }

    public static Mock<IPredictionRepository> HasFootballAPIUrl(this Mock<IPredictionRepository> repo)
    {
      repo.Setup(x => x.GetFootballAPIURL(It.IsAny<int>(), It.IsAny<int>()))
          .Returns((int a, int b) =>
            new Uri(string.Format("http://www.dectech.org/cgi-bin/new_site/GetEuroIntlSimulatedFast.pl?homeID={0}&awayType=0&awayID={1}&homeType=0&neutral=0",
              a.ToString(),
              b.ToString())));
      return repo;
    }

    public static Mock<IPredictionRepository> HasTodaysMatchesURL(this Mock<IPredictionRepository> repo)
    {
      repo.Setup(x => x.GetTodaysMatchesURL())
          .Returns(() => new Uri("http://tennisbetting365.com/api/gettodaysmatches"));
      return repo;
    }
  }
}
