using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess.Mapping
{
  public class OutcomeCommentMap : EntityTypeConfiguration<OutcomeComment>
  {
    public OutcomeCommentMap()
    {
      this.ToTable("OutcomeComments");
      this.Property(t => t.Id).HasColumnName("OutcomeCommentID_pk").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

      //this.Property(t => t.Comment).IsRequired(); //causing all sorts of havoc with automatic migrations. Come back to this
      
    }
  }
}
