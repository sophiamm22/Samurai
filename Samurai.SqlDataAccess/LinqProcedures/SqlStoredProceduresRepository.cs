using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using Infrastructure.Data;
using Samurai.Domain.Entities;
using Samurai.SqlDataAccess.Contracts;

namespace Samurai.SqlDataAccess.LinqProcedures
{
  public partial class SqlStoredProceduresRepository : GenericRepository, IStoredProceduresRepository
  {
    public SqlStoredProceduresRepository(DbContext context)
      :base(context)
    { }
  }
}
