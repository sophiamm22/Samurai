using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Repository
{
  public interface IWebRepositoryProviderAsync
  {
    IWebRepositoryAsync CreateWebRepository(DateTime repositoryDate);
  }

  public class WebRepositoryProviderAsync : IWebRepositoryProviderAsync
  {
    public IWebRepositoryAsync CreateWebRepository(DateTime repositoryDate)
    {
      throw new NotImplementedException();
    }
  }
}
