using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Castle.Windsor;

using Samurai.Services.Contracts;
using Samurai.Web.ViewModels;
using Samurai.Web.ViewModels.Football;
using Samurai.Web.ViewModels.Tennis;


namespace Samurai.Sandbox
{
  public class SamuraiConsole
  {
    private readonly IFootballFacadeService footballService;
    private readonly ITennisFacadeService tennisService;

    public SamuraiConsole(IFootballFacadeService footballService, ITennisFacadeService tennisService)
    {
      if (footballService == null) throw new ArgumentNullException("footballService");
      if (tennisService == null) throw new ArgumentNullException("tennisService");

      this.footballService = footballService;
      this.tennisService = tennisService;

    }

    public void SamuraiMenu()
    {
      while (true)
      {
        Console.WriteLine("Value-Samurai -- Main Menu");
        Console.WriteLine("Select from the list below..");
        Console.WriteLine("----------------------------");
        Console.WriteLine("1.\tTennis console");
        Console.WriteLine("2.\tFootball console");
        Console.WriteLine("");
        Console.WriteLine("3.\tExit");
        Console.WriteLine("----------------------------");
        var numberString = Console.ReadLine();
        int number;
        if (!int.TryParse(numberString, out number))
        {
          Console.WriteLine("You fucking moron!");
        }
        else
        {
          if (number == 1)
          {
            var tennisConsole = new TennisConsole(tennisService);
            tennisConsole.TennisMenu();
          }
          else if (number == 2)
          {
            var footballConsole = new FootballConsole(footballService);
            footballConsole.FootballMenu();
          }
          else
            break;
        }
      }
    }

  }
}
