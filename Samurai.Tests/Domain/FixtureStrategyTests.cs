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
    protected M.Mock<IStoredProceduresRepository> storedProcRepository;
    protected M.Mock<IFixtureRepository> fixtureRepository;
    protected List<Match> fixtures;
    protected DateTime couponDate;

    protected override void Establish_context()
    {
      base.Establish_context();
      this.fixtureRepository = new M.Mock<IFixtureRepository>();
      this.storedProcRepository = new M.Mock<IStoredProceduresRepository>();
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
      this.footballFixtureStrategy = new FootballFixtureStrategy(fixtureRepository.Object, storedProcRepository.Object, webRepository);

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
      this.fixtures.FirstOrDefault(f => f.TeamsPlayerA.Name == "Sunderland" && f.TeamsPlayerB.Name == "Newcastle").ShouldNotBeNull();
      this.fixtures.FirstOrDefault(f => f.TeamsPlayerA.Name == "QPR" && f.TeamsPlayerB.Name == "Everton").ShouldNotBeNull();

      this.fixtures.First(f => f.TeamsPlayerA.Name == "Sunderland" && f.TeamsPlayerB.Name == "Newcastle").MatchDate.ShouldEqual((new DateTime(2012, 10, 21)).AddHours(13).AddMinutes(30));
      this.fixtures.First(f => f.TeamsPlayerA.Name == "QPR" && f.TeamsPlayerB.Name == "Everton").MatchDate.ShouldEqual((new DateTime(2012, 10, 21)).AddHours(16));
    }

  }

  public class and_getting_fixtures_for_a_date_in_the_past : when_working_with_the_football_fixture_strategy
  {
    protected override void Establish_context()
    {
      base.Establish_context();
      this.couponDate = new DateTime(2012, 10, 20);

      this.webRepository = new WebRepositoryTestData("Football/" + this.couponDate.ToShortDateString().Replace("/", "-"));
      this.footballFixtureStrategy = new FootballFixtureStrategy(this.fixtureRepository.Object, this.storedProcRepository.Object, webRepository);

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
      this.fixtures.FirstOrDefault(f => f.TeamsPlayerA.Name == "Swansea" && f.TeamsPlayerB.Name == "Wigan").ShouldNotBeNull();
      this.fixtures.FirstOrDefault(f => f.TeamsPlayerA.Name == "Crystal Palace" && f.TeamsPlayerB.Name == "Millwall").ShouldNotBeNull();
      this.fixtures.FirstOrDefault(f => f.TeamsPlayerA.Name == "Oldham" && f.TeamsPlayerB.Name == "Leyton Orient").ShouldNotBeNull();
      this.fixtures.FirstOrDefault(f => f.TeamsPlayerA.Name == "York" && f.TeamsPlayerB.Name == "Dag and Red").ShouldNotBeNull();

      this.fixtures.First(f => f.TeamsPlayerA.Name == "Swansea" && f.TeamsPlayerB.Name == "Wigan").ObservedOutcomes.First().ScoreOutcome.ToString().ShouldEqual("2-1");
      this.fixtures.First(f => f.TeamsPlayerA.Name == "Crystal Palace" && f.TeamsPlayerB.Name == "Millwall").ObservedOutcomes.First().ScoreOutcome.ToString().ShouldEqual("2-2");
      this.fixtures.First(f => f.TeamsPlayerA.Name == "Oldham" && f.TeamsPlayerB.Name == "Leyton Orient").ObservedOutcomes.First().ScoreOutcome.ToString().ShouldEqual("2-0");
      this.fixtures.First(f => f.TeamsPlayerA.Name == "York" && f.TeamsPlayerB.Name == "Dag and Red").ObservedOutcomes.First().ScoreOutcome.ToString().ShouldEqual("3-2");
    }
  }

}
