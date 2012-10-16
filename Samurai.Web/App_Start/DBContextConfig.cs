using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Infrastructure.Data;

namespace Samurai.Web.App_Start
{
  public static class DBContextConfig
  {
    public static void RegisterDBContext()
    {
      DbContextManager.InitStorage(new SimpleDbContextStorage());
      DbContextManager.Init("Samurai_CF", new[] { @"Samurai.SqlDataAccess, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" }, true, true);
      SqlConnection.ClearAllPools();
    }
  }
}