using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBehave.Spec.NUnit;
using NUnit.Framework;
using AutoMapper;

namespace Samurai.Tests.StartUp
{
  public class should_successfully_launch_all_start_up_bits : Specification
  {
    [Test]
    public void automapper_should_be_configured_correctly()
    {
      Mapper.AssertConfigurationIsValid();
    }
  }
}
