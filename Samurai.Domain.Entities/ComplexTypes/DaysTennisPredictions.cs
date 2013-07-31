using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samurai.Domain.Entities.ComplexTypes
{
  public class DaysTennisPredictions
  {
    public int MatchID_pk { get; set; }
    public string TournamentName { get; set; }
    public int Year { get; set; }
    public string Surface { get; set; }
    public string Series { get; set; }
    public DateTime MatchDate { get; set; }
    public string PlayerASurname { get; set; }
    public string PlayerAFirstName { get; set; }
    public string PlayerBSurname { get; set; }
    public string PlayerBFirstName { get; set; }
    public int PlayerAGames { get; set; }
    public int PlayerBGames { get; set; }
    public decimal PlayerAProbability { get; set; }
    public decimal PlayerBProbability { get; set; }
    public decimal ESets { get; set; }
    public decimal EGames { get; set; }
    public decimal EPoints { get; set; }
    public string Score { get; set; }
    public decimal? Score_3_0 { get; set; }
    public decimal? Score_3_1 { get; set; }
    public decimal? Score_3_2 { get; set; }
    public decimal? Score_2_3 { get; set; }
    public decimal? Score_1_3 { get; set; }
    public decimal? Score_0_3 { get; set; }
    public decimal? Score_2_0 { get; set; }
    public decimal? Score_2_1 { get; set; }
    public decimal? Score_1_2 { get; set; }
    public decimal? Score_0_2 { get; set; }
  }


}
