using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;

namespace Samurai.Tests.TestInfrastructure
{
  public class FakeUnitOfWork : IUnitOfWork
  {
    public bool IsInTransaction
    {
      get { return false; }
    }

    public void SaveChanges()
    {
      
    }

    public void SaveChanges(System.Data.Objects.SaveOptions saveOptions)
    {
      
    }

    public void BeginTransaction()
    {

    }

    public void BeginTransaction(System.Data.IsolationLevel isolationLevel)
    {

    }

    public void RollBackTransaction()
    {

    }

    public void CommitTransaction()
    {

    }

    public void Dispose()
    {

    }
  }
}
