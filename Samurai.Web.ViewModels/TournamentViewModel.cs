using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Web.ViewModels
{
  public class TournamentViewModel
  {
    public int TournamentID { get; set; }
    public string TournamentName { get; set; }
    public string Slug { get; set; }
    public string Location { get; set; }
  }
}
