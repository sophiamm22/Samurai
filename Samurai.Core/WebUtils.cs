using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;

namespace Samurai.Core
{
  public static partial class WebUtils
  {
    public static IEnumerable<PropertyInfo> PropertyInfos<T>()
    {
      var t = typeof(T);
      PropertyInfo[] pi = t.GetProperties();
      foreach (var p in pi)
        yield return p;
    }

    public static IEnumerable<T> GetAndConvertJsonWebRequest<T>(string jsonString, Action<string> report,
      WebProxy proxy = null)
    {
      return JsonConvert.DeserializeObject<List<T>>(jsonString);
    }

    public static IEnumerable<T> GetAndConvertJsonWebRequest<T>(Uri uri, Action<string> report,
      WebProxy proxy = null)
    {
      var jsonString = WebUtils.GetWebpages(new Uri[] { uri }, report, s => new StreamReader(s).ReadToEnd(), proxy)
                            .First();

      var returnList = JsonConvert.DeserializeObject<List<T>>(jsonString);

      return returnList;
    }

    public static T GetAndConvertSingleJsonWebRequest<T>(Uri uri, Action<string> report,
      WebProxy proxy = null)
    {
      var jsonString = WebUtils.GetWebpages(new Uri[] { uri }, report, s => new StreamReader(s).ReadToEnd(), proxy)
                      .First();

      var ret = JsonConvert.DeserializeObject<T>(jsonString);

      return ret;

    }

    public static T GetAndConvertSingleJsonWebRequest<T>(string jsonString, Action<string> report,
      WebProxy proxy = null)
    {
      var ret = JsonConvert.DeserializeObject<T>(jsonString);

      return ret;
    }

    public static IList<T> ParseImage<T>(IEnumerable<Uri> imageURLs, Action<string> report)
      where T : IReadableImage, IRegexableWebsite, new()
    {
      var imageReader = new ImageReader<T>();
      var siteImages = WebScreenScrape<T>.GetWebpages(imageURLs, s => Image.FromStream(s));

      var returnList = new List<T>();

      foreach (var img in siteImages)
        returnList.Add(imageReader.ConvertReadItems(imageReader.GetBitmaps(new T(), img)));

      return returnList;
    }

    public static string FormPost(Uri postURL, string referer, string content, string contentType)
    {
      var ascii = new ASCIIEncoding();
      var postBytes = ascii.GetBytes(content);
      var httpRequest = (HttpWebRequest)WebRequest.Create(postURL);
      httpRequest.Method = "POST";
      httpRequest.Referer = postURL.PathAndQuery;
      httpRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
      httpRequest.ContentType = contentType;
      httpRequest.Host = postURL.Host;
      httpRequest.ContentLength = postBytes.Length;

      using (var postStream = httpRequest.GetRequestStream())
      {
        postStream.Write(postBytes, 0, postBytes.Length);
      }
      var response = httpRequest.GetResponse();
      using (StreamReader stream = new StreamReader(response.GetResponseStream()))
      {
        return stream.ReadToEnd().Replace('\"', 'æ');
      }
    }


    public static string FormPost(Uri postURL, Dictionary<string, string> formValues)
    {
      var sb = new StringBuilder();
      formValues.ToList().ForEach(kvp => sb.AppendFormat("{0}={1}&", kvp.Key, kvp.Value));
      var postString = sb.ToString().Substring(0, sb.ToString().Length - 1);
      var ascii = new ASCIIEncoding();
      var postBytes = ascii.GetBytes(postString);

      var httpRequest = (HttpWebRequest)WebRequest.Create(postURL);
      httpRequest.Method = "POST";
      httpRequest.Referer = "http://oddschecker.mobi/olympics/tennis/olympic-tennis-mens-singles/andy-roddick-v-martin-klizan/winner";
      httpRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
      httpRequest.ContentType = "multipart/form-data; boundary=---------------------------7dc35120104e6";
      httpRequest.Host = "oddschecker.mobi";
      httpRequest.ContentLength = postBytes.Length;

      using (var postStream = httpRequest.GetRequestStream())
      {
        postStream.Write(postBytes, 0, postBytes.Length);
      }
      var response = httpRequest.GetResponse();
      using (StreamReader stream = new StreamReader(response.GetResponseStream()))
      {
        return stream.ReadToEnd();
      }
    }

    public static T ParseJson<T>(string jsonString, Action<string> report)
      where T : IRegexableWebsite, new()
    {
      return JsonConvert.DeserializeObject<T>(jsonString);
    }

    public static IRegexableWebsite ParseJson<T>(Uri jsonURL, Action<string> report)
      where T : IRegexableWebsite, new()
    {
      var jsonObject = WebScreenScrape<T>.GetWebpages<T>(new List<Uri>() { jsonURL },
        s =>
        {
          using (StreamReader sr = new StreamReader(s, Encoding.UTF8))
          {
            string str = sr.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(str);
          }
        });

      return jsonObject.Cast<IRegexableWebsite>().First();
    }

    public static IList<IRegexableWebsite> ParseWebsite<T, U, V>(Uri rootURI, Action<string> report)
      where T : IRegexableWebsite, new()
      where U : IRegexableWebsite, new()
      where V : IRegexableWebsite, new()
    {
      var initialScreenscrape = new WebScreenScrape<T>(rootURI);
      initialScreenscrape.PopulateData();
      var html = initialScreenscrape.RootURLText;
      var secondScreenScrape = new WebScreenScrape<U>(html);
      secondScreenScrape.PopulateData();
      var thirdScreenScrape = new WebScreenScrape<V>(html);
      thirdScreenScrape.PopulateData();

      var allRegexables = new List<IRegexableWebsite>();

      allRegexables.AddRange(initialScreenscrape.SiteData.Cast<IRegexableWebsite>());
      allRegexables.AddRange(secondScreenScrape.SiteData.Cast<IRegexableWebsite>());
      allRegexables.AddRange(thirdScreenScrape.SiteData.Cast<IRegexableWebsite>());

      allRegexables.ForEach(a => a.Clean());

      return allRegexables
        .OrderBy(r => r.Identifier)
        .ToList();
    }

    public static IList<IRegexableWebsite> ParseWebsite<T, U>(Uri rootURI, Action<string> report, CookieContainer cookies = null)
      where T : IRegexableWebsite, new()
      where U : IRegexableWebsite, new()
    {
      var initialScreenscrape = new WebScreenScrape<T>(rootURI);
      initialScreenscrape.PopulateData(cookies);
      var html = initialScreenscrape.RootURLText;
      var secondScreenScrape = new WebScreenScrape<U>(html);
      secondScreenScrape.PopulateData(cookies);

      var allRegexables = new List<IRegexableWebsite>();

      allRegexables.AddRange(initialScreenscrape.SiteData.Cast<IRegexableWebsite>());
      allRegexables.AddRange(secondScreenScrape.SiteData.Cast<IRegexableWebsite>());

      allRegexables.ForEach(a => a.Clean());

      return allRegexables
        .OrderBy(r => r.Identifier)
        .ToList();
    }

    public static IList<IRegexableWebsite> ParseWebsite<T>(Uri rootURI, Action<string> report, CookieContainer cookies = null)
      where T : IRegexableWebsite, new()
    {
      var initialScreenscrape = new WebScreenScrape<T>(rootURI);
      initialScreenscrape.PopulateData(cookies);

      var allRegexables = new List<IRegexableWebsite>();

      allRegexables.AddRange(initialScreenscrape.SiteData.Cast<IRegexableWebsite>());

      allRegexables.ForEach(a => a.Clean());

      return allRegexables
        .OrderBy(r => r.Identifier)
        .ToList();
    }

    public static IList<IRegexableWebsite> ParseWebsite<T, U, V>(string rootURLText, Action<string> report)
      where T : IRegexableWebsite, new()
      where U : IRegexableWebsite, new()
      where V : IRegexableWebsite, new()
    {
      var initialScreenscrape = new WebScreenScrape<T>(rootURLText);
      initialScreenscrape.PopulateData();
      var secondScreenScrape = new WebScreenScrape<U>(rootURLText);
      secondScreenScrape.PopulateData();
      var thirdScreenScrape = new WebScreenScrape<V>(rootURLText);
      thirdScreenScrape.PopulateData();

      var allRegexables = new List<IRegexableWebsite>();

      allRegexables.AddRange(initialScreenscrape.SiteData.Cast<IRegexableWebsite>());
      allRegexables.AddRange(secondScreenScrape.SiteData.Cast<IRegexableWebsite>());
      allRegexables.AddRange(thirdScreenScrape.SiteData.Cast<IRegexableWebsite>());

      allRegexables.ForEach(a => a.Clean());

      return allRegexables
        .OrderBy(r => r.Identifier)
        .ToList();
    }

    public static IList<IRegexableWebsite> ParseWebsite<T, U>(string rootURLText, Action<string> report)
      where T : IRegexableWebsite, new()
      where U : IRegexableWebsite, new()
    {
      var initialScreenscrape = new WebScreenScrape<T>(rootURLText);
      initialScreenscrape.PopulateData();
      var secondScreenScrape = new WebScreenScrape<U>(rootURLText);
      secondScreenScrape.PopulateData();

      var allRegexables = new List<IRegexableWebsite>();

      allRegexables.AddRange(initialScreenscrape.SiteData.Cast<IRegexableWebsite>());
      allRegexables.AddRange(secondScreenScrape.SiteData.Cast<IRegexableWebsite>());

      allRegexables.ForEach(a => a.Clean());

      return allRegexables
        .OrderBy(r => r.Identifier)
        .ToList();
    }

    public static IList<IRegexableWebsite> ParseWebsite<T>(string rootURLText, Action<string> report)
      where T : IRegexableWebsite, new()
    {
      var initialScreenscrape = new WebScreenScrape<T>(rootURLText);
      initialScreenscrape.PopulateData();

      var allRegexables = new List<IRegexableWebsite>();

      allRegexables.AddRange(initialScreenscrape.SiteData.Cast<IRegexableWebsite>());

      allRegexables.ForEach(a => a.Clean());

      return allRegexables
        .OrderBy(r => r.Identifier)
        .ToList();
    }

    public static IEnumerable<U> GetWebpages<U>(IEnumerable<Uri> uris, Action<string> progress,
      Func<Stream, U> convert, WebProxy proxy = null)
    {
      var runningTasks = new List<Task<U>>();
      foreach (var uri in uris)
      {
        Uri URLforThis = uri;
        var wreq = (HttpWebRequest)WebRequest.Create(uri);
        ((HttpWebRequest)wreq).UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.4 (KHTML, like Gecko) Chrome/5.0.375.125 Safari/533.4";

        wreq.Proxy = proxy;
        wreq.Timeout = 1000000;

        var taskResp = Task.Factory.FromAsync<WebResponse>(wreq.BeginGetResponse, wreq.EndGetResponse, null);
        var taskResult
          = taskResp.ContinueWith(
          tsk =>
          {
            Exception firstEx = null;
            Stream webStream = null;
            U converted = default(U);

            for (int i = 0; i < 5; i++)
            {
              try
              {
                webStream = tsk.Result.GetResponseStream();
                firstEx = null;
                converted = convert(webStream);
                break;
              }
              catch (Exception ex)
              {
                if (firstEx == null)
                {
                  firstEx = ex;
                }
                progress(string.Format("Attempt #{0} failed for {1}", i.ToString(), URLforThis.ToString()));
                Thread.Sleep(1000 * (i + 1));
              }
            }
            if (firstEx != null)
            {
              progress(string.Format("Attempt failed completely for {0}", URLforThis.ToString()));
              throw new Exception("Failed to download page", firstEx);
            }
            progress(string.Format("Downloaded stream for URL:{0}", URLforThis));

            return converted;
          });
        runningTasks.Add(taskResult);
      }
      try
      {
        Task.WaitAll(runningTasks.ToArray());
      }
      catch (AggregateException aes)
      {
        foreach (var ae in aes.InnerExceptions)
        {
          progress(ae.Message);
        }
      }
      IEnumerable<U> results = runningTasks.Where(tsk => tsk.IsCompleted).Select(tsk => tsk.Result);

      return results;
    }

    public static double RowSum(this double[,] value, int index)
    {
      double result = 0;
      for (int i = 0; i <= value.GetUpperBound(1); i++)
      {
        result += value[index, i];
      }
      return result;
    }

    public static double ColumnSum(this double[,] value, int index)
    {
      double result = 0;
      for (int i = 0; i <= value.GetUpperBound(0); i++)
      {
        result += value[i, index];
      }
      return result;
    }
  }
}





