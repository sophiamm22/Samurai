using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels;

namespace Samurai.WebPresentationModel.Messaging.Fixtures.Messages
{
  public class ShowFootballFixtureReply : IReply
  {
    public FootballFixtureViewModel FootballFixture { get; set; }
    public bool Success { get; set; }
    public IDictionary<string, string> ModelErrors { get; set; }
  }
}
