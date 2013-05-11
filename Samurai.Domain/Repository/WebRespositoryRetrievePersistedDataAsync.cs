using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Samurai.Domain.Repository
{
  public class WebRespositoryRetrievePersistedDataAsync : WebRepositoryPersistDataAsync
  {
    public WebRespositoryRetrievePersistedDataAsync(string basePath)
      : base(basePath)
    {
    }

    public override async Task<string> GetHTML(Uri uri, string identifier = null)
    {
      var html = await RetirievePersisted(GetPath(uri.PathAndQuery + (identifier == null ? string.Empty : identifier)) + ".txt");
      return html;
    }

    public override async Task<T> ParseJson<T>(Uri jsonUri, string identifier = null)
    {
      var jsonString = await RetirievePersisted(GetPath(jsonUri.PathAndQuery + (identifier == null ? string.Empty : identifier)) + ".txt");
      return ParseJson<T>(jsonString);
    }

    private async Task<string> RetirievePersisted(string path)
    {
      using (var reader = new StreamReader(path))
      {
        return await reader.ReadToEndAsync();
      }
    }
  }
}
