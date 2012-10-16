using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using Samurai.Domain.Repository;
//using Model = Samurai.Domain.Model;

namespace Samurai.SqlDataAccess
{
  //public class PredictionRepository : IPredictionRepository
  //{
  //  public Model.Fund GetFundDetails(string fundName)
  //  {
  //    var fund = new Model.Fund()
  //    {
  //      FundName = fundName,
  //    };

  //    if (fundName == "Tennis")
  //    {
  //      fund.Sport = Model.Sport.Tennis;
  //      fund.Bank = 500.0 - 2.41;
  //      fund.Edge = 0.1;
  //      fund.KellyMultiplier = .25;
  //      fund.CompetitionsInFund = new List<Model.Competition>()
  //      {
  //        Model.Competition.ATP
  //      };
  //    }
  //    else if (fundName == "Premier")
  //    {
  //      fund.Sport = Model.Sport.Football;
  //      fund.Bank = 500.0 + 29.37;
  //      fund.Edge = 0.1;
  //      fund.KellyMultiplier = .25;
  //      fund.CompetitionsInFund = new List<Model.Competition>()
  //      {
  //        Model.Competition.PremierLeague
  //      };
  //    }
  //    else if (fundName == "Football League")
  //    {
  //      fund.Sport = Model.Sport.Football;
  //      fund.Bank = 500.0 + 13.63;
  //      fund.Edge = 0.1;
  //      fund.KellyMultiplier = .25;
  //      fund.CompetitionsInFund = new List<Model.Competition>()
  //      {
  //        Model.Competition.Championship,
  //        Model.Competition.LeagueOne,
  //        Model.Competition.LeagueTwo
  //      };
  //    }
  //    return fund;
  //  }

  //  public Uri GetFootballAPIURL(int teamAID, int teamBID)
  //  {
  //    return new Uri(string.Format("http://www.dectech.org/cgi-bin/new_site/GetEuroIntlSimulatedFast.pl?homeID={0}&awayType=0&awayID={1}&homeType=0&neutral=0",
  //      teamAID, teamBID));
  //  }
  //  public Uri GetTodaysMatchesURL()
  //  {
  //    return new Uri("http://www.tennisbetting365.com/api/gettodaysmatches");
  //  }
  //  public IEnumerable<Model.FootballMatch> GetDaysFootballMatches(Model.Competition comptetition, DateTime gameDate)
  //  {
  //    var matchesDTO = new List<Model.FootballMatch>();
  //    using (var db = new SamuraiEntities())
  //    {
  //      var matches = db.GetDaysMatches(gameDate, (int)comptetition)
  //                      .ToList();
  //      foreach (var match in matches)
  //      {
  //        var teamA = new Model.FootballTeam()
  //        {
  //          TeamName = match.HomeTeam,
  //          FinkTankID = match.HomeID
  //        };
  //        var teamB = new Model.FootballTeam()
  //        {
  //          TeamName = match.AwayTeam,
  //          FinkTankID = match.AwayID
  //        };
  //        var matchDTO = new Model.FootballMatch()
  //        {
  //          TeamA = teamA,
  //          TeamB = teamB,
  //          TeamAID = teamA.FinkTankID,
  //          TeamBID = teamB.FinkTankID
  //        };
  //        matchesDTO.Add(matchDTO);
  //      }
  //    }
  //    return matchesDTO;
  //  }
  //  public string GetPredictionCompetitionAlias(string predictionName, Model.OddsSource source)
  //  {
  //    if (predictionName == "Rogers Cup")
  //    {
  //      return source == Model.OddsSource.BestBetting ? "Rogers Cup" : "ATP Toronto";
  //    }
  //    else if (predictionName == "Western & Southern Open")
  //    {
  //      return source == Model.OddsSource.BestBetting ? "Western &amp; Southern Open" : "ATP Cincinnati";
  //    }
  //    else if (predictionName == "US Open")
  //    {
  //      return source == Model.OddsSource.BestBetting ? "Men's" : "Mens US Open";
  //    }
  //    //else if (predictionName == "Winston-Salem Open")
  //    //{
  //    //  return source == OddsSource.BestBetting ? 
  //    //}
  //    else
  //    {
  //      if (predictionName == "PremierLeague")
  //        return source == Model.OddsSource.BestBetting ? "Barclays Premier League" : "Premier League";
  //      else if (predictionName == "Championship")
  //        return source == Model.OddsSource.BestBetting ? "Npower Football League Championship" : "Championship";
  //      else if (predictionName == "LeagueOne")
  //        return source == Model.OddsSource.BestBetting ? "Npower Football League One" : "League 1";
  //      else if (predictionName == "LeagueTwo")
  //        return source == Model.OddsSource.BestBetting ? "Npower Football League Two" : "League 2";
  //      else
  //        throw new ArgumentException();
  //    }
  //  }
  //  public int GetGamesRequiredForBet()
  //  {
  //    return 70;
  //  }
  //  public double GetOverroundRequired(Model.Sport sport)
  //  {
  //    return 1.1;
  //  }
  //}
}
