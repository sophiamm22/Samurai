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
    Competition GetCompetition(string competitionName);
    Bookmaker FindByName(string bookmakerName);
    Bookmaker AddOrUpdate(Bookmaker bookmaker);
  }
}
