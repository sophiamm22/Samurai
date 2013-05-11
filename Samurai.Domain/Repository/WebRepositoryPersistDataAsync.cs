using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Samurai.Domain.Repository
{
  public class WebRepositoryPersistDataAsync : WebRepositoryAsync
  {
    protected readonly string basePath;
    public WebRepositoryPersistDataAsync(string basePath)
    {
      if (string.IsNullOrEmpty(basePath))
        throw new ArgumentNullException("basePath");
      this.basePath = basePath;
    }

    public override async Task<string> GetHTML(Uri uri, string identifier = null)
    {
      var html = await base.GetHTML(uri, identifier);
      await Persist(GetPath(uri.PathAndQuery + (identifier == null ? string.Empty : identifier)) + ".txt", html);
      return html;
    }

    public override async Task<IEnumerable<string>> GetHTMLs(IEnumerable<Uri> uris, string identifier = null)
    {
      var htmls = uris.Select(u => GetHTML(u, identifier));
      return await Task.WhenAll(htmls);
    }

    public override async Task<IEnumerable<T>> ParseJsons<T>(IEnumerable<Uri> jsonURIs, string identifier = null)
    {
      var jsonStrings = await base.ParseWebSitesAsync(jsonURIs, s => new StreamReader(s).ReadToEnd().Trim());
      foreach (var json in jsonURIs.Zip(jsonStrings, (x, y) => new { uri = x, str = y }))
      {
        await Persist(GetPath(json.uri.PathAndQuery + (identifier == null ? string.Empty : identifier)) + ".txt", json.str);
      }
      return jsonStrings.Select(s => ParseJson<T>(s));
    }

    public override async Task<T> ParseJson<T>(Uri jsonUri, string identifier = null)
    {
      var jsonString = await base.ParseWebSiteAsync(jsonUri, s => new StreamReader(s).ReadToEnd().Trim());
      await Persist(GetPath(jsonUri.PathAndQuery + (identifier == null ? string.Empty : identifier)) + ".txt", jsonString);
      return ParseJson<T>(jsonString);
    }

    private async Task Persist(string path, string stringToPersist)
    {
      using (var writer = new StreamWriter(path))
      {
        await writer.WriteAsync(stringToPersist);
      }
    }

    protected virtual string GetPath(string fileName)
    {
      if (!Directory.Exists(this.basePath + fileName))
        Directory.CreateDirectory(this.basePath);

      return this.basePath + @"\" + fileName.Replace("/", "æ")
                                            .Replace("?", "^")
                                            .Replace(":", "~");
    }
  }
}
