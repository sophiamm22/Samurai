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
    private readonly IPredictionRepository predictionService;
    private readonly IPredictionProvider predictionProvider;
    private readonly ICouponProvider couponProvider;

    public ValueScreener(IPredictionRepository predictionService, 
      IPredictionProvider predictionProvider, ICouponProvider couponProvider)
    {
      if (predictionService == null) throw new ArgumentNullException("predictionService");
      if (predictionProvider == null) throw new ArgumentNullException("predictionProvider");
      if (couponProvider == null) throw new ArgumentNullException("couponProvider");
      
      this.predictionService = predictionService;
      this.predictionProvider = predictionProvider;
      this.couponProvider = couponProvider;
    }


  }
}
