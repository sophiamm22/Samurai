using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class TennisPredictionStatMap : EntityTypeConfiguration<TennisPredictionStat>
  {
    public TennisPredictionStatMap()
    {
      this.ToTable("TennisPredictionStats");
      this.Property(t => t.Id).HasColumnName("MatchID_fk");

      this.Property(t => t.ESets).HasPrecision(10, 4);
      this.Property(t => t.EGames).HasPrecision(10, 4);
      this.Property(t => t.EPoints).HasPrecision(10, 4);

      // Relationships
      this.HasRequired(t => t.Match)
          .WithOptional(t => t.TennisPredictionStat)
          .WillCascadeOnDelete(true);
          
    }
  }
}
