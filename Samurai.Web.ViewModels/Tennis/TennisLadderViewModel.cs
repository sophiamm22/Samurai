using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Web.ViewModels.Tennis
{
  public class TennisLadderViewModel
  {
    public int Position { get; set; }
    public string PlayerFirstName { get; set; }
    public string PlayerSurname { get; set; }
    public string PlayerFirstNameSlug { get; set; }
    public string PlayerSurnameSlug { get; set; }
    public int? Seed { get; set; }
    public string Meta { get; set; }
  }
}
