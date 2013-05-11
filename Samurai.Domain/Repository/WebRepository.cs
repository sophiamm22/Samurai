using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.IO;

using Newtonsoft.Json;

using Samurai.Core;

namespace Samurai.Domain.Repository
{
  public interface IWebRepository
  {
    IEnumerable<string> GetHTML(IEnumerable<Uri> uris, Action<string> report, string identifier = null);
    IEnumerable<string> GetHTMLRaw(IEnumerable<Uri> uris, Action<string> report, string identifier = null);
    IEnumerable<T> GetJsonObjects<T>(Uri uri, Action<string> report, string identifier = null);
    IEnumerable<T> GetJsonObjects<T>(string jsonString, Action<string> report, string identifier = null);
    T GetJsonObject<T>(Uri uri, Action<string> report, string identifier = null) where T : IRegexableWebsite, new();
    T GetJsonObject<T>(string jsonString, Action<string> report, string identifier = null) where T : IRegexableWebsite, new();
    string FormPost(Uri postURL, Action<string> report, string referer, string content, string contentType, string identifier = null);
    IRegexableWebsite ParseJson<T>(Uri jsonURL, Action<string> report, string identifier = null) where T : IRegexableWebsite, new();
    IRegexableWebsite ParseJson<T>(string jsonString, Action<string> report, string identifier = null) where T : IRegexableWebsite, new();
  }

  public class WebRepository : IWebRepository
  {
    protected WebProxy _proxy;
    protected readonly string basePath;
    public WebRepository(string basePath)
    {
      this.basePath = basePath;
      _proxy = null;
    }

    protected virtual string GetPath(string fileName)
    {
      if (!Directory.Exists(this.basePath + fileName))
        Directory.CreateDirectory(this.basePath);

      return this.basePath + @"\" + fileName.Replace("/", "æ")
                                            .Replace("?", "^")
                                            .Replace(":", "~");
    }

    public virtual IEnumerable<string> GetHTMLRaw(IEnumerable<Uri> uris, Action<string> report, string identifier = null)
    {
      return WebUtils.GetWebpages(uris, report, s => new StreamReader(s).ReadToEnd().Trim(), _proxy);
    }

    public virtual IEnumerable<string> GetHTML(IEnumerable<Uri> uris, Action<string> report, string identifier = null)
    {
      return WebUtils.GetWebpages(uris, report, s => new StreamReader(s).ReadToEnd().Trim().Replace("\"", "æ"), _proxy);
    }

    public virtual IEnumerable<T> GetJsonObjects<T>(Uri uri, Action<string> report, string identifier = null)
    {
      return WebUtils.GetAndConvertJsonWebRequest<T>(uri, report, _proxy);
    }

    public virtual T GetJsonObject<T>(Uri uri, Action<string> report, string identifier = null)
      where T : IRegexableWebsite, new()
    {
      return WebUtils.GetAndConvertSingleJsonWebRequest<T>(uri, report, _proxy);
    }

    public virtual T GetJsonObject<T>(string jsonString, Action<string> report, string identifier = null)
      where T : IRegexableWebsite, new()
    {
      return WebUtils.GetAndConvertSingleJsonWebRequest<T>(jsonString, report);
    }

    public virtual IEnumerable<T> GetJsonObjects<T>(string jsonString, Action<string> report, string identifier = null)
    {
      return WebUtils.GetAndConvertJsonWebRequest<T>(jsonString, report, _proxy);
    }

    public virtual string FormPost(Uri postURL, Action<string> report, string referer, string content, string contentType, string identifier = null)
    {
      return WebUtils.FormPost(postURL, referer, content, contentType);
    }

    public virtual IRegexableWebsite ParseJson<T>(Uri jsonURL, Action<string> report, string identifier = null)
      where T : IRegexableWebsite, new()
    {
      return WebUtils.ParseJson<T>(jsonURL, report);
    }

    public virtual IRegexableWebsite ParseJson<T>(string jsonString, Action<string> report, string identifier = null)
      where T : IRegexableWebsite, new()
    {
      return WebUtils.ParseJson<T>(jsonString, report);
    }

  }

  public class WebRepositoryTestData : WebRepository
  {

    public WebRepositoryTestData(string basePath)
      :base(basePath)
    {
    }

    public override IEnumerable<string> GetHTMLRaw(IEnumerable<Uri> uris, Action<string> report, string identifier = null)
    {
      return GetHTML(uris, report, identifier);
    }

    public override IEnumerable<string> GetHTML(IEnumerable<Uri> uris, Action<string> report, string identifier = null)
    {
      var htmls = new List<string>();
      try
      {
        foreach (var uri in uris)
        {
          using (TextReader tr = StreamFromPathQueryIdentifier(uri.PathAndQuery + (identifier == null ? string.Empty : identifier) + ".txt", report))
          {
            htmls.Add(tr.ReadToEnd());
          }
        }
        return htmls;
      }
      catch (Exception ex)
      {
        var wex = new HttpException(404, "Fake 404: " + ex.Message);
        throw wex;
      }
    }
    public override IEnumerable<T> GetJsonObjects<T>(Uri uri, Action<string> report, string identifier = null)
    {
      using (TextReader tr = StreamFromPathQueryIdentifier(uri.PathAndQuery + (identifier == null ? string.Empty : " ID - " + identifier) + ".txt", report))
      {
        return base.GetJsonObjects<T>(tr.ReadToEnd(), report);
      }
    }

    public override IEnumerable<T> GetJsonObjects<T>(string jsonString, Action<string> report, string identifier = null)
    {
      return base.GetJsonObjects<T>(jsonString, report, identifier);
    }

    public override T GetJsonObject<T>(Uri uri, Action<string> report, string identifier = null)
    {
      using (TextReader tr = StreamFromPathQueryIdentifier(uri.PathAndQuery + (identifier == null ? string.Empty : " ID - " + identifier) + ".txt", report))
      {
        return base.GetJsonObject<T>(uri, report, identifier);
      }
    }

    public override T GetJsonObject<T>(string jsonString, Action<string> report, string identifier = null)
    {
      return base.GetJsonObject<T>(jsonString, report, identifier);
    }


    public override string FormPost(Uri postURL, Action<string> report, string referer, string content, string contentType, string identifier = null)
    {
      using (TextReader tr = StreamFromPathQueryIdentifier(postURL.PathAndQuery + (identifier == null ? string.Empty : " ID - " + identifier) + ".txt", report))
      {
        return tr.ReadToEnd();
      }
    }
    public override IRegexableWebsite ParseJson<T>(Uri jsonURL, Action<string> report, string identifier = null)
    {
      using (TextReader tr = StreamFromPathQueryIdentifier(jsonURL.PathAndQuery + (identifier == null ? string.Empty : " ID - " + identifier) + ".txt", report))
      {
        var str = tr.ReadToEnd();
        return JsonConvert.DeserializeObject<T>(str);
      }
    }

    protected virtual StreamReader StreamFromPathQueryIdentifier(string pathQueryAndIdentifier, Action<string> report)
    {
      var fileName = GetPath(pathQueryAndIdentifier);
      report(string.Format("Streaming saved URL from:{0}", fileName));
      return new StreamReader(fileName);
    }

  }

  public class WebRepositorySaveTestData : WebRepository
  {
    public WebRepositorySaveTestData(string basePath)
      :base(basePath)
    {
    }

    public override IEnumerable<string> GetHTMLRaw(IEnumerable<Uri> uris, Action<string> report, string identifier = null)
    {
      var htmls = base.GetHTMLRaw(uris, report);
      foreach (var html in htmls.Zip(uris.Select(u => u.PathAndQuery), (h, u) => new { HTML = h, UriString = u }))
      {
        using (TextWriter htmlWriter = new StreamWriter(GetPath(html.UriString + (identifier == null ? string.Empty : identifier)) + ".txt"))
        {
          htmlWriter.Write(html.HTML);
        }
      }
      return htmls;
    }

    public override IEnumerable<string> GetHTML(IEnumerable<Uri> uris, Action<string> report, string identifier = null)
    {
      var htmls = base.GetHTML(uris, report);
      foreach (var html in htmls.Zip(uris.Select(u => u.PathAndQuery), (h, u) => new { HTML = h, UriString = u }))
      {
        using (TextWriter htmlWriter = new StreamWriter(GetPath(html.UriString + (identifier == null ? string.Empty : identifier)) + ".txt"))
        {
          htmlWriter.Write(html.HTML);
        }
      }
      return htmls;
    }
    public override IEnumerable<T> GetJsonObjects<T>(Uri uri, Action<string> report, string identifier = null)
    {
      var jsonString = WebUtils.GetWebpages(new Uri[] { uri }, report, s => new StreamReader(s).ReadToEnd(), _proxy)
                               .First();
      using (TextWriter jsonWriter = new StreamWriter(GetPath(uri.PathAndQuery + (identifier == null ? string.Empty : " ID - " + identifier) + ".txt")))
      {
        jsonWriter.Write(jsonString);
      }
      return base.GetJsonObjects<T>(jsonString, report);
    }

    public override IEnumerable<T> GetJsonObjects<T>(string jsonString, Action<string> report, string identifier = null)
    {
      return base.GetJsonObjects<T>(jsonString, report, identifier);
    }

    public override T GetJsonObject<T>(Uri uri, Action<string> report, string identifier = null)
    {
      var jsonString = WebUtils.GetWebpages(new Uri[] { uri }, report, s => new StreamReader(s).ReadToEnd(), _proxy)
                               .First();

      using (TextWriter jsonWriter = new StreamWriter(GetPath(uri.PathAndQuery + (identifier == null ? string.Empty : " ID - " + identifier) + ".txt")))
      {
        jsonWriter.Write(jsonString);
      }

      return base.GetJsonObject<T>(jsonString, report, identifier);
    }

    public override T GetJsonObject<T>(string jsonString, Action<string> report, string identifier = null)
    {
      return base.GetJsonObject<T>(jsonString, report, identifier);
    }

    public override string FormPost(Uri postURL, Action<string> report, string referer, string content, string contentType, string identifier = null)
    {
      var postString = WebUtils.FormPost(postURL, referer, content, contentType);
      using (TextWriter postWriter = new StreamWriter(GetPath(postURL.PathAndQuery + (identifier == null ? string.Empty : " ID - " + identifier) + ".txt")))
      {
        postWriter.Write(postString);
      }
      return postString;
    }
    public override IRegexableWebsite ParseJson<T>(Uri jsonURL, Action<string> report, string identifier = null)
    {
      var fileName = GetPath(jsonURL.PathAndQuery + (identifier == null ? string.Empty : " ID - " + identifier) + ".txt");
      var jsonString = WebUtils.GetWebpages(new Uri[] { jsonURL }, report, s => new StreamReader(s).ReadToEnd(), _proxy)
                               .First();
      using (TextWriter jsonWriter = new StreamWriter(fileName))
      {
        jsonWriter.Write(jsonString);
      }
      return JsonConvert.DeserializeObject<T>(jsonString);
    }
  }
}
