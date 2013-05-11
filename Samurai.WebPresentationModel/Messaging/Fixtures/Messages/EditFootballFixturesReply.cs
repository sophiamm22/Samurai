using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.WebPresentationModel.Messaging.Fixtures.Messages
{
  public class EditFootballFixturesReply : IReply
  {
    public string League { get; set; }
    public string DateString { get; set; }

    public bool Success { get; set; }
    public IDictionary<string, string> ModelErrors { get; set; }
  }
}
