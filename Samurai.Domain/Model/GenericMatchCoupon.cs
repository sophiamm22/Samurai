using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samurai.Domain.Model
{
  public class GenericMatchCoupon 
  {
    public GenericMatchCoupon()
    {
      HeadlineOdds = new Dictionary<Outcome, double>();
      ActualOdds = new Dictionary<Outcome, IEnumerable<GenericOdd>>();
    }
    public string MatchIdentifier
    {
      get
      {
        var haveFirstNames = !(string.IsNullOrEmpty(FirstNameA) && string.IsNullOrEmpty(FirstNameB));
        var teamPlayerA = haveFirstNames ? string.Format("{0},{1}", TeamOrPlayerA, FirstNameA) : TeamOrPlayerA;
        var teamPlayerB = haveFirstNames ? string.Format("{0},{1}", TeamOrPlayerB, FirstNameB) : TeamOrPlayerB;

        return string.Format("{0}/vs/{1}/{2}/{3}", teamPlayerA, teamPlayerB, TournamentEventName, MatchDate.ToShortDateString().Replace("/", "-"));
      }
    }
    public int MatchId { get; set; }
    public DateTime MatchDate { get; set; }
    public string TeamOrPlayerA { get; set; }
    public string FirstNameA { get; set; }
    public string TeamOrPlayerB { get; set; }
    public string FirstNameB { get; set; }
    public string TournamentEventName { get; set; }
    public Uri MatchURL { get; set; }
    public string Source { get; set; }
    public DateTime LastChecked { get; set; }
    public bool InPlay { get; set; }
    public IDictionary<Outcome, double> HeadlineOdds { get; set; }
    public IDictionary<Outcome, IEnumerable<GenericOdd>> ActualOdds { get; set; }

    public override string ToString()
    {
      return MatchIdentifier;
    }
  }
}
