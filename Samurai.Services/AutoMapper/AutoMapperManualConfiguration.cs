using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Samurai.Services.AutoMapper;

namespace Samurai.Services.AutoMapper
{
  public class AutoMapperManualConfiguration
  {
    public static void Configure()
    {
      Mapper.Initialize(x =>
      {
        x.AddProfile<FootballCouponDictionaryProfile>();
        x.AddProfile<FootballCouponViewModelProfile>();
        x.AddProfile<FootballFixtureMatchProfile>();
        x.AddProfile<FootballFixtureViewModelProfile>();
        x.AddProfile<FootballPredictionViewModelProfile>();
        x.AddProfile<GenericMatchCouponProfile>();
        x.AddProfile<GenericMatchDetailProfile>();
        x.AddProfile<GenericOddProfile>();
        x.AddProfile<OddsSourceProfile>();
        x.AddProfile<SportProfile>();
        x.AddProfile<TennisCouponDictionaryProfile>();
        x.AddProfile<TennisCouponViewModelProfile>();
        x.AddProfile<TennisLadderViewModelProfile>();
        x.AddProfile<TennisFixtureViewModelProfile>();
        x.AddProfile<TennisMatchDetailProfile>();
        x.AddProfile<TennisMatchProfile>();
        x.AddProfile<TennisPredictionProfile>();
        x.AddProfile<TennisPredictionViewModelProfile>();
        x.AddProfile<TournamentEventViewModelProfile>();
        x.AddProfile<TournamentProfile>();
      });
    }
  }
}
