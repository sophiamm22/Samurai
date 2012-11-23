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
    private readonly IFootballFixtureService fixtureService;

    public ShowFootballFixtureHandler(IFootballFixtureService fixtureService)
      : base()
    {
      if (fixtureService == null)
        throw new ArgumentNullException("fixtureService");
      this.fixtureService = fixtureService;
    }

    public override ShowFootballFixtureReply Handle(ShowFootballFixtureRequest request)
    {
      DateTime fixtureDate = DateTime.Now.Date;
      if (!DateTime.TryParse(request.DateString, out fixtureDate))
      {
        this.reply.ModelErrors.Add("DateFormat", string.Format("Date was not in a recognised format {0}", request.DateString));
        return this.reply;
      }

      var homeTeam = this.fixtureService.GetTeamOrPlayer(request.HomeTeam);
      var awayTeam = this.fixtureService.GetTeamOrPlayer(request.AwayTeam);

      if (homeTeam == null)
      {
        this.reply.ModelErrors.Add("HomeTeamNotFound", string.Format("Team not found {0}", request.HomeTeam));
        return this.reply;
      }

      if (awayTeam == null)
      {
        this.reply.ModelErrors.Add("AwayTeamNotFound", string.Format("Team not found {0}", request.AwayTeam));
        return this.reply;
      }

      var fixture = this.fixtureService.GetFootballFixture(fixtureDate, homeTeam.TeamName, awayTeam.TeamName);

      if (fixture == null)
        return this.reply; //gets redirected to NotFound

      this.reply.FootballFixture = fixture;
      this.reply.Success = true;
      return this.reply;
    }

  }
}
