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
    protected readonly string repositoryDate;

    public ManifestWebRepository(string basePath)
      : base("Irrelevant")
    {
      this.repositoryDate = basePath;
    }

    protected override StreamReader StreamFromPathQueryIdentifier(string pathQueryAndIdentifier, Action<string> report)
    {
      var fileName = 
        pathQueryAndIdentifier.Replace("/", "æ")
                              .Replace("?", "^")
                              .Replace(":", "~");

      //I seem to have issues with upper/lower case.  No idea why but this will fix it.
      var all = Assembly.GetExecutingAssembly().GetManifestResourceNames();
      var thisOne = all.FirstOrDefault(x => string.Compare(string.Format("Samurai.Tests.TestData._{0}.{1}", this.repositoryDate, fileName), x, true) == 0);

      var manifest = Assembly.GetExecutingAssembly().GetManifestResourceStream(thisOne);
      report(string.Format("Streaming saved URL from:{0}", manifest));
      //Samurai.Tests.TestData._20130202.æfootballæfixtures-resultsæ02-february-2013.txt
      return new StreamReader(manifest);
    }

  }
}
