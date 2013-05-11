using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Domain.Entities;

namespace Samurai.Domain.Model
{
  public class FootballPrediction : GenericPrediction
  {
    public FootballPrediction()
      : base()
    {
      Sport = "Football";
    }
  }
}
