using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Repository;

namespace Samurai.Tests.TestInfrastructure
{
  public class ManifestWebRepositoryProvider : IWebRepositoryProvider
  {
    public ManifestWebRepositoryProvider()
    {
    }

    public WebRepository CreateWebRepository(DateTime repositoryDate)
    {
      return new ManifestWebRepository(repositoryDate.ToString("yyyyMMdd"));
    }
  }
}
