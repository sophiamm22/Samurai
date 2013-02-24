using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;

using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Model;
using E = Samurai.Domain.Entities;

namespace Samurai.Tests.TestInfrastructure.MockBuilders
{
  public static class BuildBookmakerRepository
  {
    public static Mock<IBookmakerRepository> Create()
    {
      return new Mock<IBookmakerRepository>();
    }

    public static Mock<IBookmakerRepository> ReturnsTournamentCouponURLs(this Mock<IBookmakerRepository> repo, IDictionary<string, Uri> hashLookupURIs)
    {
      repo.Setup(x => x.GetTournamentCouponUrl(It.IsAny<E.Tournament>(), It.IsAny<E.ExternalSource>()))
          .Returns((E.Tournament tournament, E.ExternalSource source) =>
            {
              return hashLookupURIs[tournament.TournamentName + "|" + source.Source];
            });
      return repo;
    }

  }



}
