using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using LinqToTwitter;

namespace Samurai.Sandbox
{
  public interface ITwitterClient
  {
    ITwitterAuthorizer Auth();
    string Tweet(ITwitterAuthorizer auth, string status);
  }

  public class TwitterClient
  {
    private readonly string consumerKey;
    private readonly string consumerSecret;
    private readonly string accessToken;
    private readonly string oAuthToken;

    public TwitterClient(string consumerKey, string consumerSecret, string accessToken = "", string oAuthToken = "")
    {
      if (string.IsNullOrEmpty(consumerKey) || string.IsNullOrEmpty(consumerSecret))
        throw new ArgumentNullException();
      this.consumerKey = consumerKey;
      this.consumerSecret = consumerSecret;
      this.accessToken = accessToken;
      this.oAuthToken = oAuthToken;
    }

    public ITwitterAuthorizer Auth()
    {
      var credentials = new InMemoryCredentials
      {
        ConsumerKey = this.consumerKey,
        ConsumerSecret = this.consumerSecret
      };

      if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(oAuthToken))
      {
        credentials.AccessToken = this.accessToken;
        credentials.OAuthToken = this.oAuthToken;
      }

      var auth = new PinAuthorizer
      {
        Credentials = credentials,
        UseCompression = true,
        GoToTwitterAuthorization = pageLink => Process.Start(pageLink),
        GetPin = () =>
        {
          Console.WriteLine("\nAfter you authorize this application, Twitter will give you a 7-digit PIN Number.\n");
          Console.Write("Enter the PIN number here: ");
          return Console.ReadLine();
        }
      };

      auth.Authorize();

      return auth;
    }

    public string Tweet(ITwitterAuthorizer auth, string status)
    {
      using (var twitterCtx = new TwitterContext(auth))
      {
        var tweet = twitterCtx.UpdateStatus(status);
        
        return tweet.StatusID;
      }
    } 

  }
}
