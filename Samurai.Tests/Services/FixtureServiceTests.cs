using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M = Moq;
using NBehave.Spec.NUnit;
using NUnit.Framework;

using Samurai.SqlDataAccess.Contracts;
using Samurai.Services.Contracts;
using Samurai.Services;
using Samurai.Domain.Entities;
using Samurai.Web.ViewModels;

namespace Samurai.Tests.Services
{
  public class when_working_with_the_fixture_service : Specification
  {
    protected IFixtureService fixtureService;
    protected M.Mock<IFixtureRepository> fixtureRepository;

    protected override void Establish_context()
    {
      base.Establish_context();
      this.fixtureRepository = new M.Mock<IFixtureRepository>();
      this.fixtureService = new FixtureService(this.fixtureRepository.Object);
    }
  }

  public class using_the_GetFootballFixture_method : when_working_with_the_fixture_service
  {
    protected DateTime alreadyPlayed;
    protected DateTime yetToPlay;

    protected override void Establish_context()
    {
      base.Establish_context();

      alreadyPlayed = new DateTime(2012, 09, 01);
      yetToPlay = new DateTime(2013, 01, 01);

      var manUtd = new TeamsPlayer { TeamName = "Man Utd", Slug = "man-utd" };
      var arsenal = new TeamsPlayer { TeamName = "Arsenal", Slug = "arsenal" };
      var manCity = new TeamsPlayer { TeamName = "Man City", Slug = "man-city" };

      this.fixtureRepository.Setup(r => r.GetTeamOrPlayer("man-utd")).Returns(manUtd);
      this.fixtureRepository.Setup(r => r.GetTeamOrPlayer("arsenal")).Returns(arsenal);
      this.fixtureRepository.Setup(r => r.GetTeamOrPlayer("man-city")).Returns(manCity);
      this.fixtureRepository.Setup(r => r.GetTeamOrPlayer("united-of-man")).Returns<TeamsPlayer>(null);

      var aWins = new MatchOutcome { MatchOutcomeString = "Team or player A win" };
      var oneNil = new ScoreOutcome { MatchOutcome = aWins, TeamAScore = 1, TeamBScore = 0 };
      var observedOneNil = new ObservedOutcome { ScoreOutcome = oneNil };
      var premierLeagueCompetition = new Competition { CompetitionName = "Premier League" };
      var premierLeagueTournament = new Tournament { Competition = premierLeagueCompetition, TournamentName = "Premier League" };
      var premierLeagueSeason = new TournamentEvent { EventName = "2012/13 season", Tournament = premierLeagueTournament };

      var completedMatch = new Match { TeamsPlayerA = manUtd, TeamsPlayerB = manCity, TournamentEvent = premierLeagueSeason, MatchDate = alreadyPlayed, ObservedOutcomes = new List<ObservedOutcome>() { observedOneNil } };
      var notPlayedMatch = new Match { TeamsPlayerA = manUtd, TeamsPlayerB = arsenal, TournamentEvent = premierLeagueSeason, MatchDate = yetToPlay };

      this.fixtureRepository.Setup(r => r.GetMatchFromTeamSelections(manUtd, manCity, alreadyPlayed)).Returns(completedMatch);
      this.fixtureRepository.Setup(r => r.GetMatchFromTeamSelections(manUtd, arsenal, yetToPlay)).Returns(notPlayedMatch);
    }
  }

  public class and_requesting_a_valid_fixture : using_the_GetFootballFixture_method
  {
    private FootballFixtureViewModel completedMatchViewModel;
    private FootballFixtureViewModel expectedCompletedMatchViewModel;
    private FootballFixtureViewModel notPlayedMatchViewModel;
    private FootballFixtureViewModel expectedNotPlayedMatchViewModel;
    private string alreadyPlayedDateString;
    private string yetToPlayDateString;

    protected override void Establish_context()
    {
      base.Establish_context();
      alreadyPlayedDateString = alreadyPlayed.ToString("dd-MM-yyyy");
      yetToPlayDateString = yetToPlay.ToString("dd-MM-yyyy");

      this.expectedCompletedMatchViewModel = new FootballFixtureViewModel
      {
        League = "Premier League" ,
        Season = "2012/13 season",
        MatchDate = alreadyPlayed,
        TeamsPlayerA = "Man Utd",
        TeamsPlayerB = "Man City",
        ScoreLine = "1-0"
      };

      this.expectedNotPlayedMatchViewModel = new FootballFixtureViewModel
      {
        League ="Premier League",
        Season =  "2012/13 season",
        MatchDate = yetToPlay,
        TeamsPlayerA = "Man Utd",
        TeamsPlayerB = "Arsenal",
        ScoreLine = "Not played"
      };
    }

    protected override void Because_of()
    {
      completedMatchViewModel = this.fixtureService.GetFootballFixture(alreadyPlayedDateString, "man-utd", "man-city");
      notPlayedMatchViewModel = this.fixtureService.GetFootballFixture(yetToPlayDateString, "man-utd", "arsenal");
    }

    [Test]
    public void then_a_valid_complete_match_should_be_returned()
    {
      completedMatchViewModel.PropertyValuesShouldEqual(this.expectedCompletedMatchViewModel);
    }

    [Test]
    public void then_a_valid_unplayed_match_should_be_returned()
    {
      notPlayedMatchViewModel.PropertyValuesShouldEqual(this.expectedNotPlayedMatchViewModel);
    }
  }

  public class and_requesting_an_invalid_fixture : using_the_GetFootballFixture_method
  {
    private FootballFixtureViewModel invalidTeamMatchViewModel;
    private FootballFixtureViewModel invalidDateMatchViewModel;
    private string invalidDate;

    protected override void Establish_context()
    {
      base.Establish_context();
      invalidDate = "99-99-2001";
    }

    protected override void Because_of()
    {
      var validDate = alreadyPlayed.ToString("dd-MM-yyyy");
      invalidTeamMatchViewModel = this.fixtureService.GetFootballFixture(validDate, "arsenal", "united-of-man");
      invalidDateMatchViewModel = this.fixtureService.GetFootballFixture(invalidDate, "man-utd", "man-city");
    }

    [Test]
    public void then_null_values_should_be_returned()
    {
      invalidDateMatchViewModel.ShouldBeNull();
      invalidTeamMatchViewModel.ShouldBeNull();
    }

  }



}
