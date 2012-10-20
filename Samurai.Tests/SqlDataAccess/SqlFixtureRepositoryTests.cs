using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NBehave.Spec.NUnit;
using NUnit.Framework;

using Samurai.SqlDataAccess;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.Tests.SqlDataAccess
{
  public class when_working_with_the_SQL_fixture_repository : Specification
  {
    protected IFixtureRepository fixtureRepository;

    protected override void Establish_context()
    {
      base.Establish_context();

      

    }

  }
}
