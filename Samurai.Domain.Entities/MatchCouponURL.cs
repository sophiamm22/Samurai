using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class MatchCouponURL : BaseEntity
  {
    public int MatchID { get; set; }
    public int ExternalSourceID { get; set; }
    public string MatchCouponURLString { get; set; }
    public virtual Match Match { get; set; }
    public virtual ExternalSource ExternalSource { get; set; }
  }
}
