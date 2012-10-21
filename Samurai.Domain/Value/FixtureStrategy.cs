using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly IWebRepository webRepository;

    public AbstractFixtureStrategy(IFixtureRepository fixtureRepository, IWebRepository webRepository)
    {
      this.fixtureRepository = fixtureRepository;
      this.webRepository = webRepository;
    }
    public abstract IEnumerable<Match> UpdateFixtures(DateTime fixtureDate);
    public abstract IEnumerable<Match> UpdateResults(DateTime fixtureDate, string reusedHTML = "");
  }

  public class FootballFixtureStrategy : AbstractFixtureStrategy
  {
    public FootballFixtureStrategy(IFixtureRepository fixtureRepository, IWebRepository webRepository)
      : base(fixtureRepository, webRepository)
    {
    }

    private IEnumerable<Match> ConvertFirxtures(DateTime fixtureDate, IEnumerable<ISkySportsFixture> fixtureTokens)
    {
      var returnMatches = new List<Match>();
      var skySportsSource = this.fixtureRepository.GetExternalSource("Sky Sports");
      var valueSamuraiSource = this.fixtureRepository.GetExternalSource("Value Samurai");

      foreach (var fixture in fixtureTokens)
      {
        var homeTeamName = this.fixtureRepository.GetAlias(fixture.HomeTeam, skySportsSource, valueSamuraiSource);
        var awayTeamName = this.fixtureRepository.GetAlias(fixture.AwayTeam, skySportsSource, valueSamuraiSource);

        var homeTeam = this.fixtureRepository.GetTeamOrPlayer(homeTeamName);
        var awayTeam = this.fixtureRepository.GetTeamOrPlayer(awayTeamName);

        if (homeTeam == null) throw new ArgumentNullException("homeTeam");
        if (awayTeam == null) throw new ArgumentNullException("awayTeam");

        var persistedMatch = this.fixtureRepository.GetMatchFromTeamSelections(homeTeam, awayTeam, fixtureDate);
        if (persistedMatch == null)
        {
          var league = this.fixtureRepository.GetCompetition((int)fixture.LeagueEnum);
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

      return returnMatches;
    }


    public override IEnumerable<Match> UpdateFixtures(DateTime fixtureDate)
    {
      var fixturesURL = this.fixtureRepository.GetSkySportsFootballFixturesOrResults(fixtureDate);
      var fixturesHTML = this.webRepository.GetHTML(new Uri[] { fixturesURL }, s => Console.WriteLine(s)).First();
      var fixturesTokens = WebUtils.ParseWebsite<SkySportsFootballFixture>(fixturesHTML, s => Console.WriteLine(s))
                                   .Cast<ISkySportsFixture>();

      var returnMatches = new List<Match>();

      if (fixturesTokens.Count() == 0)
        returnMatches = UpdateResults(fixtureDate, fixturesHTML).ToList();
      else
        returnMatches = ConvertFirxtures(fixtureDate, fixturesTokens).ToList();

      this.fixtureRepository.SaveChanges();

      return returnMatches;
    }

    public override IEnumerable<Match> UpdateResults(DateTime fixtureDate, string reusedHTML = "")
    {
      var fixturesURL = this.fixtureRepository.GetSkySportsFootballFixturesOrResults(fixtureDate);
      var fixturesHTML = reusedHTML == "" ? this.webRepository.GetHTML(new Uri[] { fixturesURL }, s => Console.WriteLine(s)).First() : reusedHTML;
      var fixturesTokens = WebUtils.ParseWebsite<SkySportsFootballResult>(fixturesHTML, s => Console.WriteLine(s))
                                   .Cast<ISkySportsFixture>();

      var returnMatches = new List<Match>(); 
      
      var matchAndToken = ConvertFirxtures(fixtureDate, fixturesTokens).Zip(fixturesTokens, (m, t) => new { Match = m, Token = t }).ToList();

      foreach (var mt in matchAndToken)
      {
        var match = mt.Match;
        match.ObservedOutcomes.Add(new ObservedOutcome()
        {
          Match = match,
          ScoreOutcome = this.fixtureRepository.GetScoreOutcome(mt.Token.HomeTeamScore, mt.Token.AwayTeamScore)
        });
        returnMatches.Add(match);
      }
      this.fixtureRepository.SaveChanges();
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

    public override IEnumerable<Match> UpdateResults(DateTime fixtureDate, string reusedHTML = "")
    {
      //will use but still need to implement
      throw new NotImplementedException();
    }
  }
}
