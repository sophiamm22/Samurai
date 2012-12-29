﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Samurai.Web.ViewModels.Value;

namespace Samurai.Web.ViewModels.Football
{
  public class FootballCouponViewModel
  {
    public string MatchIdentifier { get; set; }
    public string CouponURL { get; set; }
    public IEnumerable<OddViewModel> HomeWinOdds { get; set; }
    public IEnumerable<OddViewModel> DrawOdds { get; set; }
    public IEnumerable<OddViewModel> AwayOdds { get; set; }

  }
}
