using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samurai.Domain.Model
{
  public enum OddsDownloadStage
  {
    Competition,
    Matches,
    Odds
  }

  public enum Outcome
  {
    TeamOrPlayerA,
    Draw,
    TeamOrPlayerB,
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
