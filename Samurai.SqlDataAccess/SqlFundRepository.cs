using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using Infrastructure.Data;
using Samurai.SqlDataAccess.Contracts;
using Samurai.Domain.Entities;

namespace Samurai.SqlDataAccess
{
  public class SqlFundRepository : GenericRepository, IFundRepository
  {
    public SqlFundRepository(DbContext context)
      :base(context)
    { }

    public IEnumerable<Fund> GetAllFunds()
    {
      return GetQuery<Fund>();
    }

  }
}
