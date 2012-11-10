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
    Premier,
    Championship,
    League1,
    League2,
    ATP
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
}
