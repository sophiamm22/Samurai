using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoMapper;

namespace Samurai.Tests.StartUp
{
  public class StartUp
  {
    [Test]
    public void AutoMapperConfiguration()
    {
      Mapper.AssertConfigurationIsValid();
    }
  }
}
