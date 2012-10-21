using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M = Moq;
using NBehave.Spec.NUnit;
using NUnit.Framework;

using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Value;
using Samurai.Domain.Repository;
using Samurai.Domain.Entities;

namespace Samurai.Tests.Domain
{
  public class when_working_with_the_football_fixture_strategy : Specification
  {
    protected AbstractFixtureStrategy footballFixtureStrategy;
    protected IWebRepository webRepository;
    protected M.Mock<IFixtureRepository> fixtureRepository;
    protected List<Match> fixtures;
    protected DateTime couponDate;

    protected override void Establish_context()
    {
      base.Establish_context();
      this.fixtureRepository = new M.Mock<IFixtureRepository>();
      this.fixtureRepository.HasBasicMethods(this.db);
    }
  }

  public class and_getting_fixtures_for_a_date_in_the_future : when_working_with_the_football_fixture_strategy
  {
    protected override void Establish_context()
    {
      base.Establish_context();
      this.couponDate = new DateTime(2012, 10, 21);

      this.webRepository = new WebRepositoryTestData("Football/" + this.couponDate.ToShortDateString().Replace("/", "-"));
      this.footballFixtureStrategy = new FootballFixtureStrategy(fixtureRepository.Object, webRepository);

      this.fixtureRepository.HasNoPersistedMatches();
    }

    protected override void Because_of()
    {
      this.fixtures = this.footballFixtureStrategy.UpdateFixtures(this.couponDate).ToList();
    }

    [Test]
    public void then_a_complete_list_of_unplayed_fixtures_is_returned()
    {
      this.fixtures.Count().ShouldEqual(2);
      this.fixtures.FirstOrDefault(f => f.TeamsPlayerA.TeamName == "Sunderland" && f.TeamsPlayerB.TeamName == "Newcastle").ShouldNotBeNull();
      this.fixtures.FirstOrDefault(f => f.TeamsPlayerA.TeamName == "QPR" && f.TeamsPlayerB.TeamName == "Everton").ShouldNotBeNull();

      this.fixtures.First(f => f.TeamsPlayerA.TeamName == "Sunderland" && f.TeamsPlayerB.TeamName == "Newcastle").MatchDate.ShouldEqual((new DateTime(2012, 10, 21)).AddHours(13).AddMinutes(30));
      this.fixtures.First(f => f.TeamsPlayerA.TeamName == "QPR" && f.TeamsPlayerB.TeamName == "Everton").MatchDate.ShouldEqual((new DateTime(2012, 10, 21)).AddHours(16));
    }

  }

  public class and_getting_fixtures_for_a_date_in_the_past : when_working_with_the_football_fixture_strategy
  {
    protected override void Establish_context()
    {
      base.Establish_context();
      this.couponDate = new DateTime(2012, 10, 20);

      this.webRepository = new WebRepositoryTestData("Football/" + this.couponDate.ToShortDateString().Replace("/", "-"));
      this.footballFixtureStrategy = new FootballFixtureStrategy(fixtureRepository.Object, webRepository);

      this.fixtureRepository.HasPersistedMatches();
    }

    protected override void Because_of()
    {
      this.fixtures = this.footballFixtureStrategy.UpdateFixtures(this.couponDate).ToList();
    }

    [Test]
    public void the_a_complete_list_of_completed_fixtures_is_returned()
    {
      this.fixtures.Count().ShouldEqual(42);
      //spot check - one from each league
      this.fixtures.FirstOrDefault(f => f.TeamsPlayerA.TeamName == "Swansea" && f.TeamsPlayerB.TeamName == "Wigan").ShouldNotBeNull();
      this.fixtures.FirstOrDefault(f => f.TeamsPlayerA.TeamName == "Crystal Palace" && f.TeamsPlayerB.TeamName == "Millwall").ShouldNotBeNull();
      this.fixtures.FirstOrDefault(f => f.TeamsPlayerA.TeamName == "Oldham" && f.TeamsPlayerB.TeamName == "Leyton Orient").ShouldNotBeNull();
      this.fixtures.FirstOrDefault(f => f.TeamsPlayerA.TeamName == "York" && f.TeamsPlayerB.TeamName == "Dag and Red").ShouldNotBeNull();

      this.fixtures.First(f => f.TeamsPlayerA.TeamName == "Swansea" && f.TeamsPlayerB.TeamName == "Wigan").ObservedOutcomes.First().ScoreOutcome.ToString().ShouldEqual("2-1");
      this.fixtures.First(f => f.TeamsPlayerA.TeamName == "Crystal Palace" && f.TeamsPlayerB.TeamName == "Millwall").ObservedOutcomes.First().ScoreOutcome.ToString().ShouldEqual("2-2");
      this.fixtures.First(f => f.TeamsPlayerA.TeamName == "Oldham" && f.TeamsPlayerB.TeamName == "Leyton Orient").ObservedOutcomes.First().ScoreOutcome.ToString().ShouldEqual("2-0");
      this.fixtures.First(f => f.TeamsPlayerA.TeamName == "York" && f.TeamsPlayerB.TeamName == "Dag and Red").ObservedOutcomes.First().ScoreOutcome.ToString().ShouldEqual("3-2");
    }
  }

}
