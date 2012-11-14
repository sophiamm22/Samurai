using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Contracts
{
  public interface IBookmakerRepository
  {
    Uri GetTournamentCouponUrl(Tournament tournament, ExternalSource externalSource);
    ExternalSource GetExternalSource(string sourceName);
    ExternalSource GetExternalSourceFromSlug(string slug);
    Competition GetCompetition(string competitionName);
    Bookmaker FindByName(string bookmakerName);
    Bookmaker FindByOddsCheckerID(string oddsCheckerID);
    Bookmaker AddOrUpdate(Bookmaker bookmaker);
    string GetOddsCheckerJavaScript();
    void SetOddsCheckerJavaScript(string javaScript);
    string GetAlias(string bookmakerNameSource, ExternalSource source, ExternalSource destination);
  }
}
