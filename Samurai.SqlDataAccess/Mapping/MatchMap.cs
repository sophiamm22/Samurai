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
      this.Property(t => t.Id).HasColumnName("MatchID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.TournamentEventID).HasColumnName("TournamentEventID_fk");
      this.Property(t => t.TeamAID).HasColumnName("TeamAID_fk");
      this.Property(t => t.TeamBID).HasColumnName("TeamBID_fk");

      // Relationships
      this.HasRequired(t => t.TournamentEvent)
          .WithMany(t => t.Matches)
          .HasForeignKey(d => d.TournamentEventID);
      this.HasRequired(t => t.TeamsPlayerB)
          .WithMany(t => t.MatchesB)
          .HasForeignKey(d => d.TeamBID).WillCascadeOnDelete(false);
      this.HasRequired(t => t.TeamsPlayerA)
          .WithMany(t => t.MatchesA)
          .HasForeignKey(d => d.TeamAID);
      
    }
  }
}