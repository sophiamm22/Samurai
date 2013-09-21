using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Model;
using Samurai.Domain.Repository;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Exceptions;
using Samurai.Domain.Entities;
using Samurai.Core;
using Samurai.Domain.HtmlElements;

namespace Samurai.Domain.Value.Async
{
  public interface IAsyncCouponStrategy
  {
    Task<IEnumerable<IGenericTournamentCoupon>> GetTournaments(OddsDownloadStage stage = OddsDownloadStage.Tournament);
    Task<IEnumerable<GenericMatchCoupon>> GetMatches(Uri tournamentURL);
    Task<IEnumerable<GenericMatchCoupon>> GetMatches();
  }

  public abstract class AbstractAsyncCouponStrategy : IAsyncCouponStrategy
  {
    protected readonly IBookmakerRepository bookmakerRepository;
    protected readonly IFixtureRepository fixtureRepository;
    protected readonly IWebRepositoryProviderAsync webRepositoryProvider;
    protected readonly IValueOptions valueOptions;
    protected List<MissingTeamPlayerAliasObject> missingAlias;

    public AbstractAsyncCouponStrategy(IBookmakerRepository bookmakerRepository,
      IFixtureRepository fixtureRepository, IWebRepositoryProviderAsync webRepositoryProvider, 
      IValueOptions valueOptions)
    {
      if (bookmakerRepository == null) throw new ArgumentNullException("bookmakerRepository");
      if (fixtureRepository == null) throw new ArgumentNullException("fixtureRepository");
      if (webRepositoryProvider == null) throw new ArgumentNullException("webRepository");
      if (valueOptions == null) throw new ArgumentNullException("valueOptions");

      this.bookmakerRepository = bookmakerRepository;
      this.fixtureRepository = fixtureRepository;
      this.webRepositoryProvider = webRepositoryProvider;
      this.valueOptions = valueOptions;

      this.missingAlias = new List<MissingTeamPlayerAliasObject>();
    }

    public abstract Task<IEnumerable<IGenericTournamentCoupon>> GetTournaments(OddsDownloadStage stage = OddsDownloadStage.Tournament);
    public abstract Task<IEnumerable<GenericMatchCoupon>> GetMatches(Uri tournamentURL);

    public async Task<IEnumerable<GenericMatchCoupon>> GetMatches()
    {
      var couponURL = 
        this.bookmakerRepository
            .GetTournamentCouponUrl(this.valueOptions.Tournament, this.valueOptions.OddsSource);

      if (couponURL == null)
        throw new ArgumentNullException("couponURL");

      return await GetMatches(couponURL);
    }

    protected bool CheckPlayers(TeamPlayer teamOrPlayerA, TeamPlayer teamOrPlayerB, 
      string teamOrPlayerALookup, string teamOrPlayerBLookup)
    {
      bool @continue = false;
      if (teamOrPlayerA == null)
      {
        this.missingAlias.Add(new MissingTeamPlayerAliasObject
        {
          TeamOrPlayerName = teamOrPlayerALookup,
          ExternalSource = this.valueOptions.OddsSource.Source,
          ExternalSourceID = this.valueOptions.OddsSource.Id,
          Tournament = this.valueOptions.Tournament.TournamentName,
          TournamentID = this.valueOptions.Tournament.Id
        });
        @continue = true;
      }
      if (teamOrPlayerB == null)
      {
        this.missingAlias.Add(new MissingTeamPlayerAliasObject
        {
          TeamOrPlayerName = teamOrPlayerBLookup,
          ExternalSource = this.valueOptions.OddsSource.Source,
          ExternalSourceID = this.valueOptions.OddsSource.Id,
          Tournament = this.valueOptions.Tournament.TournamentName,
          TournamentID = this.valueOptions.Tournament.Id
        });
        @continue = true;
      }
      return @continue;
    }

  }

}
