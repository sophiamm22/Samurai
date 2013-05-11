using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels;

namespace Samurai.Services.Contracts
{
  public interface IAccountService
  {
    bool UserNameExists(string userName);
    void AddUser(string userName);
  }
}
