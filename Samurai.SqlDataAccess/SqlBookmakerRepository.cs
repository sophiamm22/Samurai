using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using Infrastructure.Data;
using Samurai.Domain.Entities;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.SqlDataAccess
{
  public class SqlBookmakerRepository : GenericRepository, IBookmakerRepository
  {
    public SqlBookmakerRepository(DbContext context)
      :base(context)
    { }

    public Uri GetCompetitionCouponUrl(Competition competition, ExternalSource externalSource)
    {
      var couponData = GetQuery<CompetitionCouponURL>(c => c.Id == competition.Id && c.Id == externalSource.Id)
                        .FirstOrDefault();
      if (couponData == null)
        throw new ArgumentNullException("couponData");
      else
        return new Uri(couponData.CouponURL);
    }

    public ExternalSource GetExternalSource(string source)
    {
      return First<ExternalSource>(s => s.Source == source);
    }

    public Competition GetCompetition(string competitionName)
    {
      return First<Competition>(c => c.CompetitionName == competitionName);
    }

    public IEnumerable<string> GetTeamOrPlayerAlias(string entityName, ExternalSource convertTo)
    {
      return GetQuery<TeamPlayerExternalSourceAlias>(a => a.TeamsPlayer.TeamDisplayName == entityName && a.Id == convertTo.Id)
                .Select(a => a.Alias);
    }

    public Bookmaker FindByName(string bookmakerName)
    {
      return GetQuery<Bookmaker>().Where(b => b.BookmakerName == bookmakerName)
                                  .FirstOrDefault();
    }

    public Bookmaker AddOrUpdate(Bookmaker bookmaker)
    {
      var storedBookmaker = First<Bookmaker>(b => b.BookmakerName == bookmaker.BookmakerName);
      if (storedBookmaker == null)
      {
        Add<Bookmaker>(bookmaker);
        return bookmaker;
      }
      else
      {
        Update<Bookmaker>(bookmaker);
        Save<Bookmaker>(bookmaker);
        return bookmaker;
      }
    }

  }
}
