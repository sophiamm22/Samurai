using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;

namespace Samurai.WebPresentationModel.Messaging.Fixtures.Messages
{
  public class EditFootballFixturesRequest : IRequest
  {
    public IEnumerable<FootballFixtureViewModel> Fixtures { get; set; }
  }
}
