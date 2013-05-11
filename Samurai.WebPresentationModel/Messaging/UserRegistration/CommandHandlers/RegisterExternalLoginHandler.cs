using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.WebPresentationModel.Messaging.UserRegistration.Messages;
using Samurai.Services.Contracts;

namespace Samurai.WebPresentationModel.Messaging.UserRegistration.CommandHandlers
{
  public class RegisterExternalLoginHandler : MessageHandler<RegisterExternalLoginRequest, RegisterExternalLoginReply>
  {
    private readonly IAccountService accountService;

    public RegisterExternalLoginHandler(IAccountService accountService)
      : base()
    {
      if (accountService == null)
        throw new ArgumentNullException("accountService");
      this.accountService = accountService;

    }

    public override RegisterExternalLoginReply Handle(RegisterExternalLoginRequest request)
    {
      if (this.accountService.UserNameExists(request.RegisterExternalLoginModel.UserName))
      {
        this.reply.ModelErrors.Add("UserNameExists", string.Format("Username {0} already exists. Please try a new one.", request.RegisterExternalLoginModel.UserName));
        return reply;
      }

      this.accountService.AddUser(request.RegisterExternalLoginModel.UserName);
      this.reply.Success = true;
      return this.reply;
    }

  }
}
