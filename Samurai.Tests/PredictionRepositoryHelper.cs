using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using M = Moq;
using NBehave.Spec.NUnit;
using NUnit.Framework;

using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Entities;
using Samurai.SqlDataAccess;

namespace Samurai.Tests
{
  public static partial class TestHelper
  {
    public static M.Mock<IPredictionRepository> HasBasicMethods(this M.Mock<IPredictionRepository> repo, SeedDataDictionaries db)
    {
      repo.Setup(r => r.GetFootballAPIURL(M.It.IsAny<int>(), M.It.IsAny<int>())).Returns((int teamAID, int teamBID) =>
        new Uri(string.Format("http://www.dectech.org/cgi-bin/new_site/GetEuroIntlSimulatedFast.pl?homeID={0}&awayType=0&awayID={1}&homeType=0&neutral=0",
        teamAID, teamBID)));

      repo.Setup(r => r.GetTodaysMatchesURL()).Returns(new Uri("http://www.tennisbetting365.com/api/gettodaysmatches"));

      repo.Setup(r=>r.GetTournamentAlias(M.It.Is<string>(s=> new string[] {"Rogers Cup" , "ATP Toronto", "Western &amp; Southern Open" , "ATP Cincinnati", "Men's" , "Mens US Open"}.Contains(s)), M.It.IsAny<ExternalSource>()))
        .Returns((string tournamentName, ExternalSource source) =>
      {
        throw new NotImplementedException();
      });

      return repo;
    }
  }
}
