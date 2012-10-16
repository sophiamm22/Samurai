using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class MatchMap : EntityTypeConfiguration<Match>
  {
    public MatchMap()
    {
      this.ToTable("Matches");
      this.Property(t => t.Id).HasColumnName("MatchID_pk");
      this.Property(t => t.CompetitionID).HasColumnName("CompetitionID_fk");
      this.Property(t => t.TeamAID).HasColumnName("TeamAID_fk");
      this.Property(t => t.TeamBID).HasColumnName("TeamBID_fk");

      // Relationships
      this.HasRequired(t => t.Competition)
          .WithMany(t => t.Matches)
          .HasForeignKey(d => d.CompetitionID);
      this.HasRequired(t => t.TeamsPlayerB)
          .WithMany(t => t.MatchesB)
          .HasForeignKey(d => d.TeamBID).WillCascadeOnDelete(false);
      this.HasRequired(t => t.TeamsPlayerA)
          .WithMany(t => t.MatchesA)
          .HasForeignKey(d => d.TeamAID);

    }
  }
}