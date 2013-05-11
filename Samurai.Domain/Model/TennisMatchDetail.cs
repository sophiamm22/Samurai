using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Samurai.Domain.Entities;

namespace Samurai.Domain.Model
{
  //need this as tennis fixtures/predictions come as a pair
  public class TennisMatchDetail : GenericMatchDetail
  {
    public TennisPrediction TennisPrediction { get; set; }
    public TennisPredictionStat TennisPredictionStat { get; set; }
  }
}
