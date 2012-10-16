using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class TeamsPlayerMap : EntityTypeConfiguration<TeamsPlayer>
  {
    public TeamsPlayerMap()
    {
      this.Property(t => t.TeamDisplayName).IsRequired();

      this.ToTable("TeamsPlayers");
      this.Property(t => t.Id).HasColumnName("TeamPlayerID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
      this.Property(t => t.FinkTankID).HasColumnName("FinkTankID_efk");
      this.Property(t => t.GuardianID).HasColumnName("GuardianID_efk");
      this.Property(t => t.OddscheckerID).HasColumnName("OddscheckerID_efk");
      this.Property(t => t.OddsPortalID).HasColumnName("OddsPortalID_efk");
    }
  }
}
