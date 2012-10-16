using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels;

namespace Samurai.WebPresentationModel.Messaging.UserRegistration.Messages
{
  public class RegisterExternalLoginRequest : IRequest
  {
    public RegisterExternalLoginModel RegisterExternalLoginModel { get; set; }
  }
}
