using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class TournamentCouponURL : BaseEntity
  {
    public int TournamentID { get; set; }
    public int ExternalSourceID { get; set; }
    public string CouponURL { get; set; }
    public virtual Tournament Tournament { get; set; }
    public virtual ExternalSource ExternalSource { get; set; }
  }
}
