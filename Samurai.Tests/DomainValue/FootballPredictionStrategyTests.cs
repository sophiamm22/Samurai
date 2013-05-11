using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;

using Samurai.Domain.Value;
using E = Samurai.Domain.Entities;
using Samurai.Domain.Entities.ComplexTypes;
using Samurai.Domain.APIModel;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Repository;
using Samurai.Tests.TestInfrastructure;
using Samurai.Tests.TestInfrastructure.MockBuilders;
using Model = Samurai.Domain.Model;

namespace Samurai.Tests.DomainValue
{
  public class FootballPredictionStrategyTests
  {
    public class FetchPredictions
    {
      private DateTime matchDate;
      private Mock<IFixtureRepository> mockFixtureRepository;
      private IWebRepositoryProvider webRepositoryProvider;
      private Mock<IPredictionRepository> mockPredictionRepository;
      private List<E.Match> matches;

      [Test, Category("FootballPredictionStrategyTests.FetchPredictions")]
      public void CreatesACollectionOfPredictionsForTodaysMatches()
      {
        //Arrange
        matchDate = new DateTime(2013, 02, 02);
        #region Todays matches
        matches = new List<E.Match>()
        {
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "62", Name = "QPR"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "31", Name = "Norwich" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "Premier League" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "0", Name = "Arsenal"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "39", Name = "Stoke" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "Premier League" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "7", Name = "Everton"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "1", Name = "Aston Villa" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "Premier League" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "14", Name = "Newcastle"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "6", Name = "Chelsea" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "Premier League" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "35", Name = "Reading"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "16", Name = "Sunderland" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "Premier League" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "19", Name = "West Ham"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "88", Name = "Swansea" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "Premier League" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "66", Name = "Wigan"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "15", Name = "Southampton" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "Premier League" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "8", Name = "Fulham"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "12", Name = "Man United" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "Premier League" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "2", Name = "Birmingham"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "32", Name = "Nott'm Forest" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "Championship" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "3", Name = "Blackburn"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "47", Name = "Bristol City" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "Championship" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "45", Name = "Blackpool"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "44", Name = "Barnsley" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "Championship" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "24", Name = "Crystal Palace"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "5", Name = "Charlton" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "Championship" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "25", Name = "Derby"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "53", Name = "Huddersfield" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "Championship" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "28", Name = "Ipswich"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "13", Name = "Middlesboro" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "Championship" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "9", Name = "Leeds"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "48", Name = "Cardiff" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "Championship" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "30", Name = "Millwall"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "77", Name = "Hull" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "Championship" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "59", Name = "Peterboro"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "22", Name = "Burnley" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "Championship" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "38", Name = "Sheffield Weds"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "21", Name = "Brighton" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "Championship" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "41", Name = "Watford"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "4", Name = "Bolton" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "Championship" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "71", Name = "Bury"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "95", Name = "Doncaster" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League One" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "52", Name = "Crewe"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "85", Name = "Scunthorpe" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League One" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "76", Name = "Hartlepool"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "57", Name = "Notts County" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League One" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "42", Name = "Milton Keynes"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "69", Name = "Bournemouth" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League One" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "33", Name = "Portsmouth"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "51", Name = "Colchester" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League One" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "34", Name = "Preston"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "86", Name = "Shrewsbury" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League One" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "2015", Name = "Stevenage"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "79", Name = "Leyton Orient" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League One" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "65", Name = "Tranmere"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "73", Name = "Carlisle" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League One" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "40", Name = "Walsall"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "58", Name = "Oldham" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League One" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "96", Name = "Yeovil"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "46", Name = "Brentford" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League One" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "2004", Name = "Crawley Town"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "64", Name = "Swindon" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League One" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "2029", Name = "AFC Wimbledon"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "2002", Name = "Burton" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League Two" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "49", Name = "Cheltenham"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "89", Name = "Torquay" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League Two" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "75", Name = "Exeter"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "2000", Name = "Aldershot" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League Two" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "2033", Name = "Fleetwood Town"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "20", Name = "Bradford" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League Two" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "60", Name = "Plymouth"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "2005", Name = "Dag and Red" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League Two" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "61", Name = "Port Vale"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "97", Name = "Accrington" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League Two" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "83", Name = "Rochdale"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "50", Name = "Chesterfield" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League Two" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "36", Name = "Rotherham"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "56", Name = "Northampton" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League Two" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "87", Name = "Southend"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "82", Name = "Oxford" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League Two" }}},
          new E.Match() { MatchDate = matchDate, TeamsPlayerA = new E.TeamPlayer { ExternalID = "91", Name = "York"}, TeamsPlayerB = new E.TeamPlayer { ExternalID = "2009", Name = "Morecambe" }, TournamentEvent = new E.TournamentEvent { Tournament = new E.Tournament { TournamentName = "League Two" }}}
        };
        #endregion
        this.webRepositoryProvider = new ManifestWebRepositoryProvider();

        this.mockFixtureRepository = BuildFixtureRepository.Create()
          .HasFullDaysMatchesByCompetition(matches)
          .CanGetTournamentEventFromTournamentAndDate();

        this.mockPredictionRepository = BuildPredictionRepository.Create()
          .HasFootballAPIUrl();

        var predictionStrategy = new FootballPredictionStrategy(this.mockPredictionRepository.Object,
          this.mockFixtureRepository.Object, this.webRepositoryProvider);

        var prem = new E.Tournament() { TournamentName = "Premier League" };
        var champ = new E.Tournament() { TournamentName = "Championship" };
        var league1 = new E.Tournament() { TournamentName = "League One" };
        var league2 = new E.Tournament() { TournamentName = "League Two" };

        var valueOptions = new Model.ValueOptions()
        {
          Sport = new E.Sport { SportName = "Football" },
          OddsSource = new E.ExternalSource { Source = "Not relevant" },
          CouponDate = matchDate
        };

        var allGenericPredictions = new List<Model.GenericPrediction>();

        //Act
        //Premier league
        valueOptions.Tournament = prem;
        var genericPredictionsPrem = predictionStrategy.FetchPredictions(valueOptions);
        //Championship
        valueOptions.Tournament = champ;
        var genericPredictionsChamp = predictionStrategy.FetchPredictions(valueOptions);
        //League 1
        valueOptions.Tournament = league1;
        var genericPredictionsLeague1 = predictionStrategy.FetchPredictions(valueOptions);
        //League 2
        valueOptions.Tournament = league2;
        var genericPredictionsLeague2 = predictionStrategy.FetchPredictions(valueOptions);
        //All
        allGenericPredictions.AddRange(genericPredictionsPrem);
        allGenericPredictions.AddRange(genericPredictionsChamp);
        allGenericPredictions.AddRange(genericPredictionsLeague1);
        allGenericPredictions.AddRange(genericPredictionsLeague2);

        //Assert
        allGenericPredictions.ForEach(x => 
          {
            Assert.AreEqual(x.ScoreLineProbabilities.Count(), 121);
            Assert.AreEqual(x.OutcomeProbabilities.Sum(o => o.Value), 1, 0.01);
          });
        Assert.AreEqual(genericPredictionsPrem.Count(), 8);
        Assert.AreEqual(genericPredictionsChamp.Count(), 11);
        Assert.AreEqual(genericPredictionsLeague1.Count(), 11);
        Assert.AreEqual(genericPredictionsLeague2.Count(), 10);
      }
    }
  } 
}
