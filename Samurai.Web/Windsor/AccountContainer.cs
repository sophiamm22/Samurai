using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Windsor;

using Samurai.Domain.Contracts;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.Web.Windsor
{
  public class AccountContainer : IAccountContainer
  {
    private readonly IWindsorContainer container;

    public AccountContainer(IWindsorContainer container)
    {
      this.container = container;
    }
    public IMVCMembershipRepository ResolveMembershipService()
    {
      return this.container.Resolve<IMVCMembershipRepository>();
    }
  }
}