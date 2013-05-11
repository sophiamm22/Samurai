using System;
using System.Collections.Generic;

namespace Samurai.Domain.Entities
{
  public class CompetitionsInFund
  {
    public int CompetitionInFundID_pk { get; set; }
    public int FundID_fk { get; set; }
    public int CompetitionID_fk { get; set; }
    public virtual Competition Competition { get; set; }
    public virtual Fund Fund { get; set; }
  }
}
