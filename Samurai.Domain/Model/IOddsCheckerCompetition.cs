using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Core;

namespace Samurai.Domain.Model
{
  public interface IOddsCheckerCompetition : IRegexableWebsite
  {
    string CompetitionName { get; set; }
    string PartURL { get; set; }
    Uri CompetitionURL { get; set; }
    string ConvertTeamOrPlayerName(string teamOrPlayer);
    string CompetitionType { get; set; }
  }
}
