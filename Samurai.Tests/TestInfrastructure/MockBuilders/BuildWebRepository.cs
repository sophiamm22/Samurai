using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

using Samurai.Domain.Repository;
using Samurai.Domain.APIModel;

namespace Samurai.Tests.TestInfrastructure.MockBuilders
{
  public static class BuildWebRepository
  {
    public static Mock<IWebRepository> Create()
    {
      return new Mock<IWebRepository>();
    }

    public static Mock<IWebRepository> HasSingleAPITennisTourCalendar(this Mock<IWebRepository> mockRepository)
    {
      mockRepository.Setup(r => r.GetJsonObjects<APITennisTourCalendar>(It.IsAny<Uri>(), It.IsAny<Action<string>>(), null))
                    .Returns(() =>
                      {
                        var ret = new List<APITennisTourCalendar>();
                        ret.Add(new APITennisTourCalendar
                        {
                          TournamentName = "Tóurnament Name 2013",
                          StartDate = new DateTime(2012, 12, 31),
                          EndDate = new DateTime(2013, 01, 06),
                          InProgress = true,
                          Completed = false
                        });
                        return ret;
                      });
      return mockRepository;
    }
 

  }
}
