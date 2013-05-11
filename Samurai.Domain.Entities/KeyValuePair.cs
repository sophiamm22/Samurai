using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Entities
{
  public class KeyValuePair : BaseEntity
  {
    public string Key { get; set; }
    public string Value { get; set; }
  }
}
