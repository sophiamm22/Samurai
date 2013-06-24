using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Web.API.Infrastructure
{
  public interface ISignalHandler<TCommand>
    where TCommand : class
  {
    Task Handle(TCommand command);
  }
}
