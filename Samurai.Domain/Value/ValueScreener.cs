using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.Domain.Model;
using Samurai.SqlDataAccess.Contracts;


namespace Samurai.Domain.Value
{
  public class ValueScreener
  {
    private readonly IPredictionProvider predictionProvider;
    private readonly ICouponProvider couponProvider;
    private readonly IOddsProvider oddsProvider;

    public ValueScreener(IPredictionProvider predictionProvider, ICouponProvider couponProvider,
      IOddsProvider oddsProvider)
    {
      if (predictionProvider == null) throw new ArgumentNullException("predictionProvider");
      if (couponProvider == null) throw new ArgumentNullException("couponProvider");
      if (oddsProvider == null) throw new ArgumentNullException("oddsProvider");
      
      this.predictionProvider = predictionProvider;
      this.couponProvider = couponProvider;
      this.oddsProvider = oddsProvider;
    }



  }
}
