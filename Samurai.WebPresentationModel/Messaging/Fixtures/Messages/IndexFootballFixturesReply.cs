using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;

namespace Samurai.WebPresentationModel.Messaging.Fixtures.Messages
{
  public class IndexFootballFixturesReply : IReply
  {
    public IEnumerable<FootballFixtureViewModel> FootballFixtures { get; set; }
    public bool Success { get; set; }
    public IDictionary<string, string> ModelErrors { get; set; }
  }
}
