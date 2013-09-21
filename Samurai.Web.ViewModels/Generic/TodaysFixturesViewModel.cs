using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Web.ViewModels.Football;
using Samurai.Web.ViewModels.Tennis;

namespace Samurai.Web.ViewModels.Generic
{
  public class TodaysFixturesViewModel
  {
    public DateTime MatchDate { get; set; }
    public IEnumerable<TennisFixtureViewModel> TennisFixtures { get; set; }
    public IEnumerable<FootballFixtureViewModel> FootballFixtures { get; set; }
  }
}
