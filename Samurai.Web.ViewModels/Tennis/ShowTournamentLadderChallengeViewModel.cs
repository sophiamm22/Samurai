using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Web.ViewModels.Tennis
{
  public class ShowTournamentLadderChallengeViewModel
  {
    public string ExpectedWinner { get; set; }
    public string ExpectedLoser { get; set; }
    
    public string ExpectedWinnerFirstName { get; set; }
    public string ExpectedWinnerSurname { get; set; }

    public string ExpectedLoserFirstName { get; set; }
    public string ExpectedLoserSurname { get; set; }
    
    public string Round { get; set; }
    public int RoundNumber { get; set; }
    public double Probability { get; set; }
    public int WinnersPoints { get; set; }
  }
}
