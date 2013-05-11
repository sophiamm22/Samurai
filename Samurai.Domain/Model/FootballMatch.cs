using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samurai.Domain.Model
{
  public class FootballMatch
  {
    public int TeamAID { get; set; }
    public int TeamBID { get; set; }
    public FootballTeam TeamA { get; set; }
    public FootballTeam TeamB { get; set; }
  }
}
