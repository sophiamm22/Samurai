using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Domain.Entities;

namespace Samurai.Domain.Model
{
  public class Fund
  {
    public string FundName { get; set; }
    public double KellyMultiplier { get; set; }
    public double Bank { get; set; }
    public double Edge { get; set; }
    public Sport Sport { get; set; }
    public IEnumerable<Competition> CompetitionsInFund { get; set; }
  }
}
