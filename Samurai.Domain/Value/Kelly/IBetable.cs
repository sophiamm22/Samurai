using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Value.Kelly
{
  public interface IBetable
  {
    double Probability { get; }
    double Odds { get; }
    double BookmakerImpliedProbability { get; }
    double Edge { get; }
    double SingleKellyStake { get; set; }
    double AdjustedKellyStake { get; set; }
  }

}
