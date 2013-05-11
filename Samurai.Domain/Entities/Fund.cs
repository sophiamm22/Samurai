using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class Fund : BaseEntity
  {
    public Fund()
    {
      this.Competitions = new List<Competition>();
    }

    //public int FundID_pk { get; set; }
    public string FundName { get; set; }
    public decimal Bank { get; set; }
    public decimal Turnover { get; set; }
    public decimal Revenue { get; set; }
    public decimal EdgeRequiredOverride { get; set; }
    public decimal KellyMultiplier { get; set; }

    public virtual ICollection<Competition> Competitions { get; set; }
  }
}
