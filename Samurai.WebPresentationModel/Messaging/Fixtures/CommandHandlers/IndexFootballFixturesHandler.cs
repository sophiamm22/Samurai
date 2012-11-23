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
    private readonly IFootballFixtureService fixtureService;

    public IndexFootballFixturesHandler(IFootballFixtureService fixtureService)
      : base()
    {
      if (fixtureService == null)
        throw new ArgumentNullException("fixtureService");
      this.fixtureService = fixtureService;
    }

    public override IndexFootballFixturesReply Handle(IndexFootballFixturesRequest request)
    {
      var fixtures = new List<FootballFixtureSummaryViewModel>();

      DateTime fixtureDate = DateTime.Now.Date;
      if (!request.GameWeek.HasValue && !DateTime.TryParse(request.DateString, out fixtureDate))
      {
        this.reply.ModelErrors.Add("DateFormat", string.Format("Date was not in a recognised format {0}", request.DateString));
        return this.reply;
      }
      
      var leagueViewModel = this.fixtureService.GetTournament(request.League);
      if (leagueViewModel == null)
      {
        this.reply.ModelErrors.Add("LeagueName", string.Format("Couldn't find league: {0}", request.League));
        return this.reply;
      }

      if (!request.GameWeek.HasValue)
        fixtures.AddRange(this.fixtureService.GetFootballFixturesByDate(fixtureDate, leagueViewModel.TournamentName));
      else
        fixtures.AddRange(this.fixtureService.GetFootballFixturesByGameweek((int)request.GameWeek, leagueViewModel.TournamentName));
      
      if (fixtures.Count() == 0)
      {
        this.reply.ModelErrors.Add("NoMatches", string.Format("Couldn't find {0} league matches on {1}", request.League, request.DateString));
        return this.reply;
      }

      this.reply.FootballFixtures = fixtures;
      this.reply.Success = true;
      return this.reply;
    }

  }
}
