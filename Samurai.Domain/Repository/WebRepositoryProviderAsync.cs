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
    private readonly string repositoryType;
    private readonly string basePath;

    public WebRepositoryProviderAsync(string repositoryType, string basePath)
    {
      if (string.IsNullOrEmpty(repositoryType)) throw new ArgumentNullException("repositoryType");
      if (string.IsNullOrEmpty(basePath)) throw new ArgumentNullException("folderName");
      this.repositoryType = repositoryType;
      this.basePath = basePath;
    }

    public IWebRepositoryAsync CreateWebRepository(DateTime repositoryDate)
    {
      if (this.repositoryType == "SaveTestData")
        return new WebRepositoryPersistDataAsync(basePath + @"\" + repositoryDate.ToString("yyyyMMdd"));
      else
        return new WebRepositoryAsync();
    }
  }
}
