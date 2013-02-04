using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Reflection;

using Samurai.Domain.Repository;

namespace Samurai.Tests.TestInfrastructure
{
  public class ManifestWebRepository : WebRepositoryTestData
  {
    public ManifestWebRepository()
      : base("Irrelevant")
    {
    }

    protected override StreamReader StreamFromPathQueryIdentifier(string pathQueryAndIdentifier, Action<string> report)
    {
      var fileName = pathQueryAndIdentifier.Replace("/", "æ").Replace("?", "^").Replace(":", "~");

      var manifest = Assembly.GetExecutingAssembly().GetManifestResourceStream("Samurai.Tests.TestData." + fileName);
      report(string.Format("Streaming saved URL from:{0}", manifest));

      return new StreamReader(manifest);
    }

  }
}
