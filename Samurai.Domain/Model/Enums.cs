using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samurai.Domain.Model
{
  public enum OddsDownloadStage
  {
    Tournament,
    Matches,
    Odds
  }

  public enum Outcome
  {
    Draw = 1,
    AwayWin = 2,
    HomeWin = 3,
    NotAssigned
  }

  public enum LeagueEnum
  {
    Premier = 1,
    Championship = 2,
    League1 = 3,
    League2 = 4,
    ATP = 5
  }

  public enum SourceEnum
  {
    BestBetting,
    OddsCheckerMobi,
    OddsCheckerWeb
  }

  public enum SportEnum
  {
    Football,
    Tennis
  }

  public enum ReporterImportance
  {
    Low,
    Medium,
    High,
    Error,
    Completed
  }
}
