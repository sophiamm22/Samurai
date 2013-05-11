using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

using Samurai.Domain.Repository;

namespace Samurai.Tests.TestInfrastructure.MockBuilders
{
  public static class BuildWebRepositoryProvider
  {
    public static Mock<IWebRepositoryProvider> Create()
    {
      return new Mock<IWebRepositoryProvider>();
    }

    public static Mock<IWebRepositoryProvider> ReturnsSpecificWebRepository(this Mock<IWebRepositoryProvider> repo, 
      IWebRepository webRepository)
    {
      repo.Setup(x => x.CreateWebRepository(It.IsAny<DateTime>()))
          .Returns(() => webRepository);
      return repo;
    }

  }
}
