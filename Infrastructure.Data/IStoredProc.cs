using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data
{
  public interface IStoredProc
  {
    [NotMapped]
    string StoredProcName { get; }
  }
}
