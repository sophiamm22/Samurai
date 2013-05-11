using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;

using Infrastructure.Data;
using Infrastructure.Data.Specification;

using Samurai.Domain.Entities;
using Samurai.SqlDataAccess;

namespace Samurai.Tests.TestInfrastructure
{
  public class FakeRepository : IRepository
  {
    private Dictionary<Type, ICollection<BaseEntity>> allEntities;
    private Dictionary<Type, int> currentPrimaryKey;

    private ICollection<BettingPAndL> bettingPAndLs = new SafeCollection<BettingPAndL>();
    private ICollection<Bookmaker> bookmakers = new SafeCollection<Bookmaker>();
    private ICollection<TournamentCouponURL> tournamentCouponURLs = new SafeCollection<TournamentCouponURL>();
    private ICollection<CompetitionCouponURL> competitionCouponURLs = new SafeCollection<CompetitionCouponURL>();
    private ICollection<Competition> competitions = new SafeCollection<Competition>();
    private ICollection<Tournament> tournaments = new SafeCollection<Tournament>();
    private ICollection<TournamentEvent> tournamentEvents = new SafeCollection<TournamentEvent>();
    private ICollection<ExternalSource> externalSources = new SafeCollection<ExternalSource>();
    private ICollection<Fund> funds = new SafeCollection<Fund>();
    private ICollection<MatchCouponURL> matchCouponURLs = new SafeCollection<MatchCouponURL>();
    private ICollection<Match> matches = new SafeCollection<Match>();
    private ICollection<MatchOutcomeOdd> matchOutcomeOdds = new SafeCollection<MatchOutcomeOdd>();
    private ICollection<MatchOutcomeProbabilitiesInMatch> matchOutcomeProbabilitiesInMatches = new SafeCollection<MatchOutcomeProbabilitiesInMatch>();
    private ICollection<MatchOutcome> matchOutcomes = new SafeCollection<MatchOutcome>();
    private ICollection<ObservedOutcome> observedOutcomes = new SafeCollection<ObservedOutcome>();
    private ICollection<ScoreOutcomeProbabilitiesInMatch> scoreOutcomeProbabilitiesInMatches = new SafeCollection<ScoreOutcomeProbabilitiesInMatch>();
    private ICollection<ScoreOutcome> scoreOutcomes = new SafeCollection<ScoreOutcome>();
    private ICollection<Sport> sports = new SafeCollection<Sport>();
    private ICollection<TeamPlayerExternalSourceAlias> teamPlayerExternalSourceAliass = new SafeCollection<TeamPlayerExternalSourceAlias>();
    private ICollection<TeamPlayer> teamsPlayers = new SafeCollection<TeamPlayer>();
    private ICollection<BookmakerExternalSourceAlias> bookmakerExternalSourceAliass = new SafeCollection<BookmakerExternalSourceAlias>();
    private ICollection<KeyValuePair> keyValuePairs = new SafeCollection<KeyValuePair>();
    private ICollection<TennisPredictionStat> tennisPredictionStats = new SafeCollection<TennisPredictionStat>();

    public FakeRepository()
    {
      currentPrimaryKey = new Dictionary<Type, int>();

      var seedData = new SeedData();
      var fields = 
        typeof(FakeRepository)
        .GetFields(BindingFlags.NonPublic|BindingFlags.Instance);
      foreach (var field in fields)
      {
        
      }
    }

    public TEntity GetByKey<TEntity>(object keyValue) where TEntity : BaseEntity
    {
      var entityCollection = (ICollection<TEntity>)this.allEntities[typeof(TEntity)];
      return First<TEntity>(x => x.Id == (int)keyValue); // bit dodgy but all my keys are integers
    }

    public IQueryable<TEntity> GetQuery<TEntity>() where TEntity : BaseEntity
    {
      var entityCollection = (ICollection<TEntity>)this.allEntities[typeof(TEntity)];
      return entityCollection.AsQueryable();
    }

    public IQueryable<TEntity> GetQuery<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : BaseEntity
    {
      return GetQuery<TEntity>().Where(predicate);
    }

    public IQueryable<TEntity> GetQuery<TEntity>(ISpecification<TEntity> criteria) where TEntity : BaseEntity
    {
      return criteria.SatisfyingEntitiesFrom(GetQuery<TEntity>());
    }

    public TEntity Single<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : BaseEntity
    {
      return GetQuery<TEntity>().SingleOrDefault<TEntity>(criteria);
    }

    public TEntity Single<TEntity>(ISpecification<TEntity> criteria) where TEntity : BaseEntity
    {
      return criteria.SatisfyingEntityFrom(GetQuery<TEntity>());
    }

    public TEntity First<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : BaseEntity
    {
      return GetQuery<TEntity>().FirstOrDefault(predicate);
    }

    public TEntity First<TEntity>(ISpecification<TEntity> criteria) where TEntity : BaseEntity
    {
      return criteria.SatisfyingEntitiesFrom(GetQuery<TEntity>()).FirstOrDefault();
    }

    public void Add<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
      if (entity == null) throw new ArgumentNullException("entity");
      this.allEntities[typeof(TEntity)].Add(entity);
    }

    public void Attach<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
      throw new NotImplementedException();
    }

    public void Delete<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
      this.allEntities[typeof(TEntity)].Remove(entity);
    }

    public void Delete<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : BaseEntity
    {
      IEnumerable<TEntity> records = Find<TEntity>(criteria);
      foreach (TEntity record in records)
      {
        Delete<TEntity>(record);
      }
    }

    public void Delete<TEntity>(ISpecification<TEntity> criteria) where TEntity : BaseEntity
    {
      IEnumerable<TEntity> records = Find<TEntity>(criteria);
      foreach (TEntity record in records)
      {
        Delete<TEntity>(record);
      }
    }

    public void Update<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
      //all referenced in memory so shouldn't matter
    }

    public IEnumerable<TEntity> Find<TEntity>(ISpecification<TEntity> criteria) where TEntity : BaseEntity
    {
      return criteria.SatisfyingEntitiesFrom(GetQuery<TEntity>());
    }

    public IEnumerable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : BaseEntity
    {
      return GetQuery<TEntity>().Where(criteria);
    }

    public TEntity FindOne<TEntity>(ISpecification<TEntity> criteria) where TEntity : BaseEntity
    {
      return criteria.SatisfyingEntityFrom(GetQuery<TEntity>());
    }

    public TEntity FindOne<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : BaseEntity
    {
      return GetQuery<TEntity>().Where(criteria).FirstOrDefault();
    }

    public IEnumerable<TEntity> GetAll<TEntity>() where TEntity : BaseEntity
    {
      return GetQuery<TEntity>().AsEnumerable();
    }

    public IEnumerable<TEntity> Get<TEntity, TOrderBy>(Expression<Func<TEntity, TOrderBy>> orderBy, int pageIndex, int pageSize, SortOrder sortOrder = SortOrder.Ascending) where TEntity : BaseEntity
    {
      if (sortOrder == SortOrder.Ascending)
      {
        return GetQuery<TEntity>().OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsEnumerable();
      }
      return GetQuery<TEntity>().OrderByDescending(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsEnumerable();
    }

    public IEnumerable<TEntity> Get<TEntity, TOrderBy>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TOrderBy>> orderBy, int pageIndex, int pageSize, SortOrder sortOrder = SortOrder.Ascending) where TEntity : BaseEntity
    {
      if (sortOrder == SortOrder.Ascending)
      {
        return GetQuery<TEntity>(criteria).OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsEnumerable();
      }
      return GetQuery<TEntity>(criteria).OrderByDescending(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsEnumerable();
    }

    public IEnumerable<TEntity> Get<TEntity, TOrderBy>(ISpecification<TEntity> specification, Expression<Func<TEntity, TOrderBy>> orderBy, int pageIndex, int pageSize, SortOrder sortOrder = SortOrder.Ascending) where TEntity : BaseEntity
    {
      if (sortOrder == SortOrder.Ascending)
      {
        return specification.SatisfyingEntitiesFrom(GetQuery<TEntity>()).OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsEnumerable();
      }
      return specification.SatisfyingEntitiesFrom(GetQuery<TEntity>()).OrderByDescending(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsEnumerable();
    }

    public int Count<TEntity>() where TEntity : BaseEntity
    {
      return GetQuery<TEntity>().Count();
    }

    public int Count<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : BaseEntity
    {
      return GetQuery<TEntity>().Count(criteria);
    }

    public int Count<TEntity>(ISpecification<TEntity> criteria) where TEntity : BaseEntity
    {
      return criteria.SatisfyingEntitiesFrom(GetQuery<TEntity>()).Count();
    }

    public IUnitOfWork UnitOfWork
    {
      get 
      {
        if (unitOfWork == null)
        {
          unitOfWork = new FakeUnitOfWork();
        }
        return unitOfWork;
      }
    }

    private IUnitOfWork unitOfWork;
  }
}
