using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public static M.Mock<IBookmakerRepository> HasBasicMethods(this M.Mock<IBookmakerRepository> repo, SeedDataDictionaries db)
    {
      repo.Setup(r => r.GetTournamentCouponUrl(M.It.IsAny<Tournament>(), M.It.IsAny<ExternalSource>()))
          .Returns((Tournament tournament, ExternalSource externalSource) =>
        {
          var lookup = tournament.TournamentName + "|" + externalSource.Source;
          Uri returnURI = null;
          switch (lookup)
          {
            case "Premier League|Best Betting": returnURI = new Uri("http://odds.bestbetting.com/football/england/premier-league/"); break;
            case "Championship|Best Betting": returnURI = new Uri("http://odds.bestbetting.com/football/england/football-league-championship/"); break;
            case "League One|Best Betting": returnURI = new Uri("http://odds.bestbetting.com/football/england/league-one/"); break;
            case "League Two|Best Betting": returnURI = new Uri("http://odds.bestbetting.com/football/england/league-two/"); break;
            case "ATP|Best Betting": returnURI = new Uri("http://odds.bestbetting.com/tennis/"); break;

            case "Premier League|Odds Checker Mobi": returnURI = new Uri("http://oddschecker.mobi/football/english/premier-league"); break;
            case "Championship|Odds Checker Mobi": returnURI = new Uri("http://oddschecker.mobi/football/english/championship"); break;
            case "League One|Odds Checker Mobi": returnURI = new Uri("http://oddschecker.mobi/football/english/league-1"); break;
            case "League Two|Odds Checker Mobi": returnURI = new Uri("http://oddschecker.mobi/football/english/league-2"); break;
            case "ATP|Odds Checker Mobi": returnURI = new Uri("http://oddschecker.mobi/tennis/mens-tour/"); break;

            case "Premier League|Odds Checker Web": returnURI = new Uri("http://www.oddschecker.com/football/english/premier-league"); break;
            case "Championship|Odds Checker Web": returnURI = new Uri("http://www.oddschecker.com/football/english/championship"); break;
            case "League One|Odds Checker Web": returnURI = new Uri("http://www.oddschecker.com/football/english/league-1"); break;
            case "League Two|Odds Checker Web": returnURI = new Uri("http://www.oddschecker.com/football/english/league-2"); break;
            case "ATP|Odds Checker Web": returnURI = new Uri("http://www.oddschecker.com/tennis/mens-tour"); break;

            default: throw new ArgumentException("Competition & External source");
          }
          return returnURI;
        });
      return repo;
    }
  }
}
