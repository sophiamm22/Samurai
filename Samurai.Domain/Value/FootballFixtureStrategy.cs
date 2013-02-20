﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Core;
using Samurai.Domain.HtmlElements;

namespace Samurai.Domain.Value
{
  public interface IFootballFixtureStrategy
  {
    IEnumerable<GenericMatchDetailQuery> UpdateFixtures(DateTime fixtureDate);
    IEnumerable<GenericMatchDetailQuery> UpdateResults(DateTime fixtureDate);
  }

  public class FootballFixtureStrategy : IFootballFixtureStrategy
  {
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly IStoredProceduresRepository storedProcRepository;
    protected readonly IWebRepositoryProvider webRepositoryProvider;

    protected string storedHTML = "";


    public FootballFixtureStrategy(IFixtureRepository fixtureRepository, IStoredProceduresRepository storedProcRepository,
      IWebRepositoryProvider webRepositoryProvider)
    {
      this.fixtureRepository = fixtureRepository;
      this.storedProcRepository = storedProcRepository;
      this.webRepositoryProvider = webRepositoryProvider;
    }

    public IEnumerable<GenericMatchDetailQuery> UpdateFixtures(DateTime fixtureDate)
    {
      var fixturesURL = this.fixtureRepository.GetSkySportsFootballFixturesOrResults(fixtureDate);

      var webRepository = this.webRepositoryProvider.CreateWebRepository(fixtureDate);
      
      var fixturesHTML = 
        string.IsNullOrEmpty(this.storedHTML) ? webRepository.GetHTML(new Uri[] { fixturesURL }, s => Console.WriteLine(s)).First() : this.storedHTML;

      var fixturesTokens = 
        WebUtils.ParseWebsite<SkySportsFootballFixture>(fixturesHTML, s => Console.WriteLine(s))
                .Cast<ISkySportsFixture>();

      var returnMatches = new List<GenericMatchDetailQuery>();

      var matchAndToken = 
         ConvertFixtures(fixtureDate, fixturesTokens).ToList();

      this.fixtureRepository.SaveChanges();
      return this.storedProcRepository
                 .GetGenericMatchDetails(fixtureDate, "Football")
                 .ToList();
    }

    public IEnumerable<GenericMatchDetailQuery> UpdateResults(DateTime fixtureDate)
    {
      var fixturesURL = this.fixtureRepository.GetSkySportsFootballFixturesOrResults(fixtureDate);

      var webRepository = this.webRepositoryProvider.CreateWebRepository(fixtureDate);

      var fixturesHTML = string.IsNullOrEmpty(this.storedHTML) ? webRepository.GetHTML(new Uri[] { fixturesURL }, s => Console.WriteLine(s), "results").First() : this.storedHTML;
      var fixturesTokens = WebUtils.ParseWebsite<SkySportsFootballResult>(fixturesHTML, s => Console.WriteLine(s))
                                   .Cast<ISkySportsFixture>();

      var matchAndToken = ConvertFixtures(fixtureDate, fixturesTokens).Zip(fixturesTokens, (m, t) => new { Match = m, Token = t }).ToList();

      foreach (var mt in matchAndToken)
      {
        var match = mt.Match;
        if (match.ObservedOutcomes.Count() == 0)
        {
          match.ObservedOutcomes.Add(new ObservedOutcome()
          {
            Match = match,
            ScoreOutcome = this.fixtureRepository.GetScoreOutcome(mt.Token.HomeTeamScore, mt.Token.AwayTeamScore)
          });
        }
        else
        {
          match.ObservedOutcomes.First().ScoreOutcome = this.fixtureRepository.GetScoreOutcome(mt.Token.HomeTeamScore, mt.Token.AwayTeamScore);
        }
      }
      this.fixtureRepository.SaveChanges();
      return this.storedProcRepository
                 .GetGenericMatchDetails(fixtureDate, "Football")
                 .ToList();
    }

    private IEnumerable<Match> ConvertFixtures(DateTime fixtureDate, IEnumerable<ISkySportsFixture> fixtureTokens)
    {
      var returnMatches = new List<Match>();
      var skySportsSource = this.fixtureRepository.GetExternalSource("Sky Sports");
      var valueSamuraiSource = this.fixtureRepository.GetExternalSource("Value Samurai");
      var sport = this.fixtureRepository.GetSport("Football");

      foreach (var fixture in fixtureTokens)
      {
        var homeTeam = this.fixtureRepository.GetAlias(fixture.HomeTeam, skySportsSource, valueSamuraiSource, sport);
        var awayTeam = this.fixtureRepository.GetAlias(fixture.AwayTeam, skySportsSource, valueSamuraiSource, sport);

        if (homeTeam == null) throw new ArgumentNullException("homeTeam");
        if (awayTeam == null) throw new ArgumentNullException("awayTeam");

        var persistedMatch = this.fixtureRepository.GetMatchFromTeamSelections(homeTeam, awayTeam, fixtureDate);
        if (persistedMatch == null)
        {
          var tournamentEvent = this.fixtureRepository.GetFootballTournamentEvent((int)fixture.LeagueEnum, fixtureDate);
          var newMatch = new Match()
          {
            TournamentEvent = tournamentEvent,
            MatchDate = fixtureDate.AddHours(fixture.KickOffHours).AddMinutes(fixture.KickOffMintutes),
            TeamsPlayerA = homeTeam,
            TeamsPlayerB = awayTeam,
            EligibleForBetting = true
          };

          returnMatches.Add(newMatch);
          this.fixtureRepository.AddMatch(newMatch);
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

  }
}