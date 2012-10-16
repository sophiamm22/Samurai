using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Samurai.Domain.Entities;
using Model = Samurai.Domain.Model;

namespace Samurai.Domain.Repository
{
  public interface IFundRepository
  {
    IEnumerable<Fund> GetAllFunds();
  }
}
