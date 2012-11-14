using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;

namespace Samurai.Sandbox
{
  class Program
  {
    static void Main(string[] args)
    {
      var container = new WindsorContainer();
      container.Install(new SamuraiSandboxWindsorInstaller());

      

    }
  }
}
