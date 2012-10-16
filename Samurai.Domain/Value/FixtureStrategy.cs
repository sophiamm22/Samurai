using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model = Samurai.Domain.Model;
using Samurai.Domain.Entities;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.HtmlElements;
using Samurai.Core;

namespace Samurai.Domain.Value
{
  public abstract class AbstractFixtureStrategy
  {
    protected readonly IFixtureRepository fixtureService;
    protected readonly IWebRepository webRepository;

    public AbstractFixtureStrategy(IFixtureRepository fixtureService, IWebRepository webRepository)
    {
      this.fixtureService = fixtureService;
      this.webRepository = webRepository;
    }
    public abstract IEnumerable<Match> UpdateFixtures(DateTime fixtureDate);
    public abstract IEnumerable<Match> UpdateResults(DateTime fixtureDate);
  }

  public class FootballFixtureStrategy : AbstractFixtureStrategy
  {
    public FootballFixtureStrategy(IFixtureRepository fixtureService, IWebRepository webRepository)
      : base(fixtureService, webRepository)
    {
    }

    public override IEnumerable<Match> UpdateFixtures(DateTime fixtureDate)
    {
      var fixturesURL = this.fixtureService.GetSkySportsFootballFixturesOrResults(fixtureDate);
      var fixturesHTML = this.webRepository.GetHTML(new Uri[] { fixturesURL }, s => Console.WriteLine(s)).First();
      var fixturesTokens = WebUtils.ParseWebsite<SkySportsFootballFixture>(fixturesHTML, s => Console.WriteLine(s))
                                   .Cast<SkySportsFootballFixture>();

      if (fixturesTokens.Count() == 0) return UpdateResults(fixtureDate);

      var returnMatches = new List<Match>();

      foreach (var fixture in fixturesTokens)
      {
        var homeTeam = this.fixtureService.GetTeamFromSkySportsName(fixture.HomeTeam);
        var awayTeam = this.fixtureService.GetTeamFromSkySportsName(fixture.AwayTeam);

        if (homeTeam == null) throw new ArgumentNullException("homeTeam");
        if (awayTeam == null) throw new ArgumentNullException("awayTeam");

        var persistedMatch = this.fixtureService.GetFootballFixtureFromTeamSelections(homeTeam, awayTeam, fixtureDate);
        if (persistedMatch == null)
        {
          var league = this.fixtureService.GetCompetition((int)fixture.LeagueEnum);
          var newMatch = new Match()
          {
            Competition = league,
            MatchDate = fixtureDate.AddHours(fixture.KickOffHours).AddMinutes(fixture.KickOffMintutes),
            TeamsPlayerA = homeTeam,
            TeamsPlayerB = awayTeam,
            EligibleForBetting = true
          };

          returnMatches.Add(newMatch);
        }
        else
        {
          //only field we're likley to need to update
          persistedMatch.MatchDate = fixtureDate.AddHours(fixture.KickOffHours).AddMinutes(fixture.KickOffMintutes);
          returnMatches.Add(persistedMatch);
        }
      }
      this.fixtureService.SaveChanges();
      return returnMatches;
    }

    public override IEnumerable<Match> UpdateResults(DateTime fixtureDate)
    {
      var fixturesURL = this.fixtureService.GetSkySportsFootballFixturesOrResults(fixtureDate);
      var fixturesHTML = this.webRepository.GetHTML(new Uri[] { fixturesURL }, s => Console.WriteLine(s)).First();
      var fixturesTokens = WebUtils.ParseWebsite<SkySportsFootballResult>(fixturesHTML, s => Console.WriteLine(s))
                                   .Cast<SkySportsFootballResult>();

      var returnMatches = new List<Match>();

      foreach (var fixture in fixturesTokens)
      {
        var homeTeam = this.fixtureService.GetTeamFromSkySportsName(fixture.HomeTeam);
        var awayTeam = this.fixtureService.GetTeamFromSkySportsName(fixture.AwayTeam);

        if (homeTeam == null) throw new ArgumentNullException("homeTeam");
        if (awayTeam == null) throw new ArgumentNullException("awayTeam");

        var persistedMatch = this.fixtureService.GetFootballFixtureFromTeamSelections(homeTeam, awayTeam, fixtureDate);
        if (persistedMatch == null)
        {
          var league = this.fixtureService.GetCompetition((int)fixture.LeagueEnum);
          var newMatch = new Match()
          {
            Competition = league,
            MatchDate = fixtureDate.AddHours(fixture.KickOffHours).AddMinutes(fixture.KickOffMintutes),
            TeamsPlayerA = homeTeam,
            TeamsPlayerB = awayTeam,
            EligibleForBetting = true
          };

          newMatch.ObservedOutcomes.Add(new ObservedOutcome()
          {
            ScoreOutcome = this.fixtureService.GetScoreOutcome(fixture.HomeTeamScore, fixture.AwayTeamScore)
          });

          returnMatches.Add(newMatch);
        }
        else
        {
          persistedMatch.MatchDate = fixtureDate.AddHours(fixture.KickOffHours).AddMinutes(fixture.KickOffMintutes);
          var onlyMatchResult = persistedMatch.ObservedOutcomes.FirstOrDefault();
          if (onlyMatchResult != null)
            persistedMatch.ObservedOutcomes.Clear();

          persistedMatch.ObservedOutcomes.Add(new ObservedOutcome()
          {
            ScoreOutcome = this.fixtureService.GetScoreOutcome(fixture.HomeTeamScore, fixture.AwayTeamScore)
          });

          returnMatches.Add(persistedMatch);
        }
      }
      this.fixtureService.SaveChanges();
      return returnMatches;
    }
  }

  public class TennisFixtureStrategy : AbstractFixtureStrategy
  {
    public TennisFixtureStrategy(IFixtureRepository fixtureService, IWebRepository webRepository)
      :base(fixtureService, webRepository)
    {
    }

    public override IEnumerable<Match> UpdateFixtures(DateTime fixtureDate)
    {
      //probably not required as predictions and fixtures come as a set
      throw new NotImplementedException("Smells like a leaky abstraction!");
    }

    public override IEnumerable<Match> UpdateResults(DateTime fixtureDate)
    {
      //will use but still need to implement
      throw new NotImplementedException();
    }
  }
}
