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

    public int? GetMinGamesForTennisBet()
    {
      return First<Competition>(t => t.Sport.SportName == "Tennis").GamesRequiredForBet;
    }

    public void AddMatchOutcomeOdd(MatchOutcomeOdd odd)
    {
      Add<MatchOutcomeOdd>(odd);
    }

    public Uri GetTournamentCouponUrl(Tournament tournament, ExternalSource externalSource)
    {
      var couponData = GetQuery<TournamentCouponURL>(c => c.Tournament.Id == tournament.Id && c.ExternalSource.Id == externalSource.Id)
                        .FirstOrDefault();
      if (couponData == null)
        return null;
      else
        return new Uri(couponData.CouponURL);
    }

    public IEnumerable<ExternalSource> GetActiveOddsSources()
    {
      return GetQuery<ExternalSource>(e => e.UseByDefault && e.OddsSource).OrderByDescending(x => x.PrescreenDecider).ToList();
    }

    public ExternalSource GetExternalSourceFromSlug(string slug)
    {
      return First<ExternalSource>(s => s.Source.Replace(" ", "-").ToLower() == slug.ToLower());
    }

    public ExternalSource GetExternalSource(string source)
    {
      return First<ExternalSource>(s => s.Source == source);
    }

    public Competition GetCompetition(string competitionName)
    {
      return First<Competition>(c => c.CompetitionName == competitionName);
    }

    public Bookmaker FindByName(string bookmakerName)
    {
      return GetQuery<Bookmaker>().Where(b => b.BookmakerName == bookmakerName)
                                  .FirstOrDefault();
    }

    public Bookmaker FindByOddsCheckerID(string oddsCheckerID)
    {
      return First<Bookmaker>(b => b.OddsCheckerShortID == oddsCheckerID);
    }

    public string GetOddsCheckerJavaScript()
    {
      var script = First<KeyValuePair>(k => k.Key == "OddsCheckerJavaScript");
      if (script == null)
        return string.Empty;
      else
        return script.Value;
    }

    public void SetOddsCheckerJavaScript(string javaScript)
    {
      var script = First<KeyValuePair>(k => k.Key == "OddsCheckerJavaScript");
      if (script == null)
        Add<KeyValuePair>(new KeyValuePair { Key = "OddsCheckerJavaScript", Value = javaScript });
      else
        script.Value = javaScript;
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

    public string GetAlias(string bookmakerNameSource, ExternalSource source, ExternalSource destination)
    {
      var bookmakerNameDestination = string.Empty;
      var bookmakerAlias = GetQuery<BookmakerExternalSourceAlias>()
                              .Include(t => t.Bookmaker)
                              .Where(a => a.Alias == bookmakerNameSource &&
                                          a.ExternalSource.Source == source.Source);

      if (bookmakerAlias.Count() == 0)
        bookmakerNameDestination = bookmakerNameSource;
      else
        bookmakerNameDestination = bookmakerAlias.First().Bookmaker.BookmakerName;

      return bookmakerNameDestination;
    }

    public void AddTournamentCouponURL(ExternalSource source, Tournament tournament, string couponURL)
    {
      var tournamentCoupon = new TournamentCouponURL 
      { 
        ExternalSource = source,
        Tournament = tournament,
        CouponURL = couponURL
      };
      Add<TournamentCouponURL>(tournamentCoupon);
      SaveChanges();
    }

    public void SaveChanges()
    {
      UnitOfWork.SaveChanges();
    }

    public IQueryable<MatchCouponURL> GetMatchCouponURLs(int matchID)
    {
      return GetQuery<MatchCouponURL>(x => x.Id == matchID);
    }

    public void AddMatchCouponURL(MatchCouponURL entity)
    {
      Add<MatchCouponURL>(entity);
    }

    public IQueryable<MatchOutcomeOdd> GetMatchOutcomeOdds(int probOutcomeID)
    {
      return GetQuery<MatchOutcomeOdd>(x => x.MatchOutcomeProbabilitiesInMatchID == probOutcomeID)
        .Include(x => x.ExternalSource)
        .Include(x => x.Bookmaker);
    }

  }
}
