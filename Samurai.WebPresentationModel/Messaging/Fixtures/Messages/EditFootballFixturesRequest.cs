using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Samurai.Web.ViewModels;

namespace Samurai.WebPresentationModel.Messaging.Fixtures.Messages
{
  public class EditFootballFixturesRequest : IRequest
  {
    public IEnumerable<FootballFixtureSummaryViewModel> Fixtures { get; set; }
  }
}
