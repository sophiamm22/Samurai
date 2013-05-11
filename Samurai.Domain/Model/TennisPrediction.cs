using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Domain.Entities;

namespace Samurai.Domain.Model
{
  public class TennisPrediction : GenericPrediction
  {
    public int PlayerAGames { get; set; }
    public int PlayerBGames { get; set; }

    public double? EPoints { get; set; }
    public double? EGames { get; set; }
    public double? ESets { get; set; }

    public TennisPrediction()
      : base()
    {
      Sport = "Tennis";
    }
  }
}
