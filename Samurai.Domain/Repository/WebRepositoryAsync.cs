using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

using Newtonsoft.Json;

using Samurai.Core;

namespace Samurai.Domain.Repository
{
  public interface IWebRepositoryAsync
  {
    Task<string> GetHTML(Uri uri, string identifier = null);
    Task<IEnumerable<string>> GetHTMLs(IEnumerable<Uri> uris, string identifier = null);

    Task<IEnumerable<T>> ParseJsonEnumerable<T>(Uri jsonUri, string identifier = null) where T : IRegexableWebsite, new();
    IEnumerable<T> ParseJsonEnumerable<T>(string jsonString) where T : IRegexableWebsite, new();
    Task<IEnumerable<T>> ParseJsons<T>(IEnumerable<Uri> jsonUris, string identifier = null) where T : IRegexableWebsite, new();
    Task<T> ParseJson<T>(Uri jsonUri, string identifier = null) where T : IRegexableWebsite, new();
    T ParseJson<T>(string jsonString) where T : IRegexableWebsite, new();

    Task<TConverted> ParseWebSiteAsync<TConverted>(Uri uri, Func<Stream, TConverted> convert);
    Task<IEnumerable<TConverted>> ParseWebSitesAsync<TConverted>(IEnumerable<Uri> uris, Func<Stream, TConverted> convert);
  }

  public class WebRepositoryAsync : IWebRepositoryAsync
  {
    public virtual async Task<string> GetHTML(Uri uri, string identifier = null)
    {
      return await ParseWebSiteAsync(uri, s => new StreamReader(s).ReadToEnd().Trim().Replace("\"", "æ"));
    }

    public virtual async Task<IEnumerable<string>> GetHTMLs(IEnumerable<Uri> uris, string identifier = null)
    {
      return await ParseWebSitesAsync(uris, s => new StreamReader(s).ReadToEnd().Trim().Replace("\"", "æ"));
    }

    public virtual async Task<IEnumerable<T>> ParseJsonEnumerable<T>(Uri jsonUri, string identifier = null) where T : IRegexableWebsite, new()
    {
      var site = await ParseWebSiteAsync(jsonUri, s => new StreamReader(s).ReadToEnd().Trim());
      return ParseJsonEnumerable<T>(site);
    }

    public virtual IEnumerable<T> ParseJsonEnumerable<T>(string jsonString) where T : IRegexableWebsite, new()
    {
      return JsonConvert.DeserializeObject<List<T>>(jsonString);
    }

    public virtual async Task<IEnumerable<T>> ParseJsons<T>(IEnumerable<Uri> jsonURIs, string identifier = null) where T : IRegexableWebsite, new()
    {
      var sites = await ParseWebSitesAsync(jsonURIs, s => new StreamReader(s).ReadToEnd().Trim());
      return sites.Select(s => ParseJson<T>(s));
    }    
    
    public virtual async Task<T> ParseJson<T>(Uri jsonUri, string identifier = null) where T : IRegexableWebsite, new()
    {
      return ParseJson<T>(await ParseWebSiteAsync(jsonUri, s => new StreamReader(s).ReadToEnd().Trim()));
    }

    public virtual T ParseJson<T>(string jsonString) where T : IRegexableWebsite, new()
    {
      return JsonConvert.DeserializeObject<T>(jsonString);
    }

    public async Task<TConverted> ParseWebSiteAsync<TConverted>(Uri uri, Func<Stream, TConverted> convert)
    {
      var webRequest = WebRequest.Create(uri);
      ((HttpWebRequest)webRequest).UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.4 (KHTML, like Gecko) Chrome/5.0.375.125 Safari/533.4";
      webRequest.Timeout = 1000000;

      var response = await webRequest.GetResponseAsync();
      using (var stream = response.GetResponseStream())
      {
        return convert(stream);
      }
    }

    public async Task<IEnumerable<TConverted>> ParseWebSitesAsync<TConverted>(IEnumerable<Uri> uris, Func<Stream, TConverted> convert)
    {
      var downloadTasks = uris.Select(u => ParseWebSiteAsync(u, convert));
      return await Task.WhenAll(downloadTasks);
    }
  }
}
