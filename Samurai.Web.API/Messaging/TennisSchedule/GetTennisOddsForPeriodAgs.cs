using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Samurai.Web.API.Messaging.TennisSchedule
{
  public class GetTennisOddsForPeriodAgs
  {
    public int StartYear { get; set; }
    public int StartMonth { get; set; }
    public int StartDay { get; set; }

    public int EndYear { get; set; }
    public int EndMonth { get; set; }
    public int EndDay { get; set; }
  }
}