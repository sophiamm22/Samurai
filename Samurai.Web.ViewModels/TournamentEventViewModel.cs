using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Web.ViewModels
{
  public class TournamentEventViewModel
  {
    public string EventName { get; set; }
    public string Slug { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool TournamentInProgress { get; set; }
    public bool TournamentCompleted { get; set; }
  }
}
