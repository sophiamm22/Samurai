using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNet.SignalR;
using Castle.Windsor;

namespace Samurai.Web.API.Windsor
{
  public class SignalRDependencyResolver : DefaultDependencyResolver
  {
    private readonly IWindsorContainer container;

    public SignalRDependencyResolver(IWindsorContainer container)
    {
      if (container == null) throw new ArgumentNullException("container");
      this.container = container;
    }

    public override object GetService(Type serviceType)
    {
      return TryGet(serviceType) ?? base.GetService(serviceType);
    }

    public override IEnumerable<object> GetServices(Type serviceType)
    {
      return TryGetAll(serviceType).Concat(base.GetServices(serviceType));
    }

    private object TryGet(Type serviceType)
    {
      try
      {
        return this.container.Resolve(serviceType);
      }
      catch (Exception)
      {
        return null;
      }
    }

    private IEnumerable<object> TryGetAll(Type serviceType)
    {
      try
      {
        var array = this.container.ResolveAll(serviceType);
        return array.Cast<object>().ToList();
      }
      catch (Exception)
      {
        return null;
      }
    }

  }
}