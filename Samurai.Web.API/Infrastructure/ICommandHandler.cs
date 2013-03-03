﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Web.API.Infrastructure
{
  public interface ICommandHandler<TMessage>
  {
    void Handle(TMessage message);
  }
}
