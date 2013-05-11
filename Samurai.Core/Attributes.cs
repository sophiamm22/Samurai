using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samurai.Core
{
  public class DataGridName : Attribute
  {
    public string GridHeading { get; set; }
    public DataGridName(string gridHeading)
    { GridHeading = gridHeading; }
  }

  public class Position : Attribute
  {
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Colour { get; set; }

    public Position(int x, int y, int width, int height, int colour)
    {
      X = x;
      Y = y;
      Height = height;
      Width = width;
      Colour = colour;
    }
  }
}
