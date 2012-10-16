using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.WebPresentationModel.Messaging.Fixtures.Messages;
using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;

namespace Samurai.WebPresentationModel.Messaging.Fixtures.CommandHandlers
{
  public class IndexFootballFixturesHandler : MessageHandler<IndexFootballFixturesRequest, IndexFootballFixturesReply>
  {
    private readonly IFixtureService fixtureService;

    public IndexFootballFixturesHandler(IFixtureService fixtureService)
      : base()
    {
      if (fixtureService == null)
        throw new ArgumentNullException("fixtureService");
      this.fixtureService = fixtureService;
    }

    public override IndexFootballFixturesReply Handle(IndexFootballFixturesRequest request)
    {
      var fixtures = new List<FootballFixtureSummaryViewModel>();
      if (!request.GameWeek.HasValue)
        fixtures.AddRange(this.fixtureService.GetFootballFixturesByDate(request.League, request.DateString));
      else
        fixtures.AddRange(this.fixtureService.GetFootballFixturesByGameweek(request.League, request.DateString));
      
      if (fixtures.Count() == 0)
      {
        if (this.fixtureService.LeagueExists(request.League))
          this.reply.ModelErrors.Add("NoMatches", string.Format("Couldn't find {0} league matches on {1}", request.League, request.DateString));
        else
          this.reply.ModelErrors.Add("NoLeague", string.Format("Didn't recognise the league name {0}", request.League));
        return this.reply;
      }

      this.reply.FootballFixtures = fixtures;
      this.reply.Success = true;
      return this.reply;
    }

  }
}
