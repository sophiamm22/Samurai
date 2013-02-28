﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Web.Messaging.Infrastructure
{
  public static class Extensions
  {
    public static bool IsValidDate(int year, int month, int day)
    {
      if (year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year)
        return false;

      if (month < 1 || month > 12)
        return false;

      return day > 0 && day <= DateTime.DaysInMonth(year, month);
    }
  }
}
