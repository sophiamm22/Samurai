using System.ComponentModel.DataAnnotations;

namespace Samurai.Web.API.Messaging.TennisSchedule
{
  public class TennisScheduleArgs
  {
    [Required]public int Day { get; set; }
    [Required]public int Month { get; set; }
    [Required]public int Year { get; set; }
  }
}
