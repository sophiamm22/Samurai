using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using Samurai.Domain.Model;
//using Samurai.Domain.Repository;

namespace Samurai.SqlDataAccess
{
  //public class BookmakerRepository : IBookmakerRepository
  //{
  //  public Uri GetCompetitionURL(Competition competition, OddsSource source)
  //  {
  //    if (competition == Competition.ATP)
  //    {
  //      if (source == OddsSource.BestBetting)
  //        return new Uri("http://odds.bestbetting.com/tennis/");
  //      else
  //        return new Uri("http://oddschecker.mobi/tennis/us-open/");
  //    }
  //    else
  //    {
  //      if (source == OddsSource.BestBetting)
  //      {
  //        var uriString = "http://odds.bestbetting.com/football/england/{0}";
  //        return new Uri(string.Format(uriString, ""));
  //      }
  //      else
  //      {
  //        var uriString = "http://oddschecker.mobi/football/english/{0}";
  //        return new Uri(string.Format(uriString, ""));
  //      }
  //    }

  //  }
  //  public string TeamOrPlayerLookup(string localTeamOrPlayerName, Sport sport, OddsSource source)
  //  {
  //    if (sport == Sport.Tennis)
  //    {
  //      if (source == OddsSource.BestBetting)
  //        return localTeamOrPlayerName.Substring(0, localTeamOrPlayerName.IndexOf(',') + 3);
  //      else
  //        return (localTeamOrPlayerName.Split(',')[1].Trim() + " " + localTeamOrPlayerName.Split(',')[0].Trim()).Replace("-", " ");
  //    }
  //    else
  //    {
  //      using (var db = new SamuraiEntities())
  //      {
  //        var team = db.FootballTeams.FirstOrDefault(t => t.Team == localTeamOrPlayerName);
  //        if (team == null) throw new Exception("Oops");
  //        return source == OddsSource.BestBetting ? team.BestBettingID_fk : team.OddscheckerID_fk;
  //      }
  //    }
  //  }
  //}
}
