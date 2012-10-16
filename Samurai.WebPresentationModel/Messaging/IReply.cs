using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.WebPresentationModel.Messaging
{
  public interface IReply
  {
    bool Success { get; set; }
    IDictionary<string, string> ModelErrors { get; set; }
  }
}
