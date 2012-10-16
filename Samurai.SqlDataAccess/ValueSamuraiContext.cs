using System.Data.Entity;
using System.Data.Entity.Infrastructure;

using Samurai.Domain.Entities;
using Samurai.SqlDataAccess.Mapping;

namespace Samurai.SqlDataAccess
{
  public class ValueSamuraiContext : DbContext
  {
    static ValueSamuraiContext()
    {
      Database.SetInitializer<ValueSamuraiContext>(null);
    }

    public ValueSamuraiContext()
      : base("Name=ValueSamuraiContext")
    {
    }

    public DbSet<BettingPAndL> BettingPAndLs { get; set; }
    public DbSet<Bookmaker> Bookmakers { get; set; }
    public DbSet<CompetitionCouponURL> CompetitionCouponURLs { get; set; }
    public DbSet<Competition> Competitions { get; set; }
    public DbSet<ExternalSource> ExternalSources { get; set; }
    public DbSet<Fund> Funds { get; set; }
    public DbSet<MatchCouponURL> MatchCouponURLs { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<MatchOutcomeOdd> MatchOutcomeOdds { get; set; }
    public DbSet<MatchOutcomeProbabilitiesInMatch> MatchOutcomeProbabilitiesInMatches { get; set; }
    public DbSet<MatchOutcome> MatchOutcomes { get; set; }
    public DbSet<ObservedOutcome> ObservedOutcomes { get; set; }
    public DbSet<ScoreOutcomeProbabilitiesInMatch> ScoreOutcomeProbabilitiesInMatches { get; set; }
    public DbSet<ScoreOutcome> ScoreOutcomes { get; set; }
    public DbSet<Sport> Sports { get; set; }
    public DbSet<TeamPlayerExternalSourceAlias> TeamPlayerExternalSourceAlias { get; set; }
    public DbSet<TeamsPlayer> TeamsPlayers { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      modelBuilder.Configurations.Add(new BettingPAndLMap());
      modelBuilder.Configurations.Add(new BookmakerMap());
      modelBuilder.Configurations.Add(new CompetitionCouponURLMap());
      modelBuilder.Configurations.Add(new CompetitionMap());
      modelBuilder.Configurations.Add(new ExternalSourceMap());
      modelBuilder.Configurations.Add(new FundMap());
      modelBuilder.Configurations.Add(new MatchCouponURLMap());
      modelBuilder.Configurations.Add(new MatchMap());
      modelBuilder.Configurations.Add(new MatchOutcomeOddMap());
      modelBuilder.Configurations.Add(new MatchOutcomeProbabilitiesInMatchMap());
      modelBuilder.Configurations.Add(new MatchOutcomeMap());
      modelBuilder.Configurations.Add(new ObservedOutcomeMap());
      modelBuilder.Configurations.Add(new ScoreOutcomeProbabilitiesInMatchMap());
      modelBuilder.Configurations.Add(new ScoreOutcomeMap());
      modelBuilder.Configurations.Add(new SportMap());
      modelBuilder.Configurations.Add(new TeamPlayerExternalSourceAliasMap());
      modelBuilder.Configurations.Add(new TeamsPlayerMap());
    }
  }
}
