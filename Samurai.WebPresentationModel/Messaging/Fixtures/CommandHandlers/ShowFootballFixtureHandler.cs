using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Samurai.WebPresentationModel.Messaging.Fixtures.Messages;
using Samurai.Services.Contracts;

namespace Samurai.WebPresentationModel.Messaging.Fixtures.CommandHandlers
{
  public class ShowFootballFixtureHandler : MessageHandler<ShowFootballFixtureRequest, ShowFootballFixtureReply>
  {
    private readonly IFixtureService fixtureService;

    public ShowFootballFixtureHandler(IFixtureService fixtureService)
      : base()
    {
      if (fixtureService == null)
        throw new ArgumentNullException("fixtureService");
      this.fixtureService = fixtureService;
    }

    public override ShowFootballFixtureReply Handle(ShowFootballFixtureRequest request)
    {
      var fixture = this.fixtureService.GetFootballFixture(request.DateString, request.HomeTeam, request.AwayTeam);

      if (fixture == null)
        return this.reply; //gets redirected to NotFound

      this.reply.FootballFixture = fixture;
      this.reply.Success = true;
      return this.reply;
    }

  }
}
