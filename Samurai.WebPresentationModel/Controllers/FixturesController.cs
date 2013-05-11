using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Samurai.WebPresentationModel.Messaging;
using Samurai.WebPresentationModel.Messaging.Fixtures.Messages;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;

namespace Samurai.WebPresentationModel.Controllers
{
  public class FixturesController : CommandController
  {
    public FixturesController(IBus bus)
      : base(bus)
    { }

    public ActionResult IndexFootballFixtures(string league, string dateString)
    {
      var request = new IndexFootballFixturesRequest { League = league, DateString = dateString };

      return Message<IndexFootballFixturesRequest, IndexFootballFixturesReply>(
        request, 
        m => View(m));
    }

    public ActionResult IndexFootballFixtures(string league, int gameWeek)
    {
      var request = new IndexFootballFixturesRequest { League = league, GameWeek = gameWeek };

      return Message<IndexFootballFixturesRequest, IndexFootballFixturesReply>(
        request,
        m => View(m));
    }

    [HttpPost]
    public ActionResult EditFootballFixtures(IEnumerable<FootballFixtureViewModel> fixtures)
    {
      var request = new EditFootballFixturesRequest { Fixtures = fixtures };

      return Message<EditFootballFixturesRequest, EditFootballFixturesReply>(
        request, 
        m => RedirectToAction<FixturesController>(a => a.IndexFootballFixtures(m.League, m.DateString)));
    }

    public ActionResult ShowFootballFixture(string dateString, string homeTeam, string awayTeam)
    {
      var request = new ShowFootballFixtureRequest { DateString = dateString, HomeTeam = homeTeam, AwayTeam = awayTeam };

      return Message<ShowFootballFixtureRequest, ShowFootballFixtureReply>(
        request, 
        m => View(m),
        m => View("NotFound"));
    }

  }
}
