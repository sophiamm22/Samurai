using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.WebPresentationModel.Messaging.Fixtures.Messages
{
  public class IndexFootballFixturesRequest : IRequest
  {
    public string League { get; set; }
    public string DateString { get; set; }
    public int? GameWeek { get; set; }
  }
}
