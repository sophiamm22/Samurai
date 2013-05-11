using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace Samurai.Core
{
  public class WebScreenScrape<T> where T : IRegexableWebsite, new()
  {
    private int _currentIdentifier = -1;

    public List<T> SiteData { get; set; }
    public Uri RootURI { get; private set; }
    public string RootURLText { get; private set; }

    public WebScreenScrape(string rootURL)
    {
      SiteData = new List<T>();
      RootURLText = rootURL;
      RootURI = null;
    }

    public WebScreenScrape(Uri rootURI)
    {
      SiteData = new List<T>();
      RootURI = rootURI;
    }

    public void PopulateData(CookieContainer cookies = null)
    {
      if (RootURI != null)
        RootURLText = GetWebpages(new Uri[] { RootURI }, s => new StreamReader(s).ReadToEnd().Trim().Replace("\"", "æ"), cookies).First();

      var regexs = (new T()).Regexs;

      foreach (var attribute in GetWebpageAttributes(RootURLText, regexs))
      {
        var commonProperties
          = GetObjectProperties(new T())
            .Intersect(attribute.Select(a => a.Key));

        var newClass = new T();
        newClass.Identifier = _currentIdentifier;

        foreach (var property in commonProperties)
        {
          object[] update = new object[1];
          update[0] = attribute[property];
          typeof(T).InvokeMember(property,
            BindingFlags.SetProperty, null, newClass, update);
        }
        SiteData.Add(newClass);
      }
    }

    private IEnumerable<string> GetObjectProperties<U>(U obj)
    {
      Type t = typeof(U);
      PropertyInfo[] propertyInfo = t.GetProperties();

      foreach (var property in propertyInfo)
      { yield return property.Name; }
    }

    private IEnumerable<Dictionary<string, string>> GetWebpageAttributes(string URL, List<Regex> regexs)
    {
      var propertiesWebPage =
        GeneratePropertiesDictionariesWebSite(URL, regexs);

      var attributes = propertiesWebPage
                          .OrderBy(item => item.Key);

      foreach (var dictItem in attributes)
      {
        _currentIdentifier = dictItem.Key;
        yield return dictItem.Value;
      }
    }

    private Dictionary<int, Dictionary<string, string>> GeneratePropertiesDictionariesWebSite(
      string fullSite, List<Regex> regexs)
    {
      var dictMatchGroups = new Dictionary<int, Dictionary<string, string>>();

      foreach (Regex regex in regexs)
      {
        MatchCollection matches = regex.Matches(fullSite);

        foreach (Match match in matches)
        {
          var dictGroups = new Dictionary<string, string>();
          if (regex.GroupNameFromNumber(1) == "Key" && regex.GroupNameFromNumber(2) == "Value")
          { dictGroups.Add(match.Groups["Key"].ToString(), match.Groups["Value"].ToString()); }
          else
          {
            var groups = regex.GetGroupNames();
            for (int g = 0; g < groups.Length; g++)
            { dictGroups.Add(groups[g], match.Groups[g].ToString()); }
          }
          dictMatchGroups.Add(match.Index, dictGroups);
        }
      }
      return dictMatchGroups;
    }

    public static CookieContainer AuthenticateUserCookies(Uri uri, CookieContainer cookies = null)
    {
      var request = (HttpWebRequest)WebRequest.Create(uri);

      request.CookieContainer = cookies == null ? new CookieContainer() : cookies;

      WebProxy proxy = new WebProxy("proxy.lb.uk.deloitte.com", true);
      proxy.Credentials = new NetworkCredential("mstaniforth", "London123");
      request.Proxy = proxy;

      var response = (HttpWebResponse)request.GetResponse();

      var cookiesRet = new CookieContainer();

      cookiesRet.Add(response.Cookies);
      return cookiesRet;
    }


    public static IEnumerable<U> GetWebpages<U>(IEnumerable<Uri> uris,
      Func<Stream, U> convert, CookieContainer cookies = null)
    {
      var runningTasks = new List<Task<U>>();
      foreach (var uri in uris)
      {
        Uri URLforThis = uri;
        var wreq = (HttpWebRequest)WebRequest.Create(uri);
        ((HttpWebRequest)wreq).UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.4 (KHTML, like Gecko) Chrome/5.0.375.125 Safari/533.4";
        if (cookies != null)
          wreq.CookieContainer = cookies;

        wreq.Timeout = 1000000;

        var taskResp = Task.Factory.FromAsync<WebResponse>(wreq.BeginGetResponse, wreq.EndGetResponse, null);
        var taskResult
          = taskResp.ContinueWith(
          tsk =>
          {
            Stream webStream = tsk.Result.GetResponseStream();
            return convert(webStream);
          });
        runningTasks.Add(taskResult);
      }
      Task.WaitAll(runningTasks.ToArray());
      IEnumerable<U> results = runningTasks.Select(tsk => tsk.Result);

      return results;
    }

  }

}
