using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Web.ViewModels.Value
{
  public class OddsSourceViewModel
  {
    public int SourceID { get; set; }
    public string Source { get; set; }
    public string SourceNotes { get; set; }
    public bool TheoreticalOddsSource { get; set; }
  }
}
