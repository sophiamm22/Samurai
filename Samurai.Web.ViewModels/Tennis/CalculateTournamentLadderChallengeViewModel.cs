using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Web.ViewModels.Tennis
{
  public class CalculateTournamentLadderChallengeViewModel
  {
    public DateTime StartDate { get; set; }
    public string Tournament { get; set; }
    public bool AllowRecalculation { get; set; }
  }
}
