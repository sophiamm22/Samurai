using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.WebPresentationModel.Messaging.Fixtures.Messages
{
  public class ShowFootballFixtureRequest : IRequest
  {
    public string DateString { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
  }
}
