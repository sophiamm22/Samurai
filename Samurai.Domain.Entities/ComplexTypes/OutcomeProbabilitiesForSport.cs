using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Entities.ComplexTypes
{
  public class OutcomeProbabilitiesForSport
  {
    public string Tournament { get; set; }
    public DateTime Date { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
    public IDictionary<int, decimal> OutcomeProbabilties { get; set; }
    public decimal EdgeRequired { get; set; }
    public int? GamesRequiredForBet { get; set; }
  }
}
