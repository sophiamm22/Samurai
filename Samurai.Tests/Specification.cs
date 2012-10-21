using NBehave.Spec.NUnit;
using NUnit.Framework;

using Samurai.SqlDataAccess;

using Samurai.Services.AutoMapper;

namespace Samurai.Tests
{
  [TestFixture]
  public class Specification : SpecBase
  {
    protected SeedDataDictionaries db;
    public override void MainSetup()
    {
      AutoMapperManualConfiguration.Configure();
      db = new SeedDataDictionaries();
      base.MainSetup();
    }
  }
}
