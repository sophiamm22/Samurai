using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.WebPresentationModel.Messaging.UserRegistration.Messages
{
  public class RegisterExternalLoginReply : IReply
  {
    public bool Success { get; set; }
    public IDictionary<string, string> ModelErrors { get; set; }
  }
}
