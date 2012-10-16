using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class MatchCouponURL : BaseEntity
  {
    public int CompetitionID { get; set; }
    public int ExternalSourceID { get; set; }
    public string MatchCouponURLString { get; set; }
    public virtual Competition Competition { get; set; }
    public virtual ExternalSource ExternalSource { get; set; }
  }
}
