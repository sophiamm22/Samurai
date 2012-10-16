using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Entities;

namespace Samurai.Domain.Repository
{
  public interface IBookmakerRepository
  {
    Uri GetCompetitionCouponUrl(Competition competition, ExternalSource externalSource);
    ExternalSource GetExternalSource(string sourceName);
    Competition GetCompetition(string competitionName);
    IEnumerable<string> GetTeamOrPlayerAlias(string entityName, ExternalSource convertTo);
    Bookmaker FindByName(string bookmakerName);
    Bookmaker AddOrUpdate(Bookmaker bookmaker);
  }
}
