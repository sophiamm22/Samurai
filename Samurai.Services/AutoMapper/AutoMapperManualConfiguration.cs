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
        x.AddProfile<FootballFixtureProfile>();
        x.AddProfile<OddsSourceProfile>();
        x.AddProfile<SportProfile>();
        x.AddProfile<TournamentProfile>();
        x.AddProfile<TennisMatchProfile>();
      });
    }
  }
}
