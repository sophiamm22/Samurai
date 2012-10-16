using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samurai.Domain.Entities
{
  public class Role
  {
    public Role()
    {
      this.Users = new List<User>();
    }
    public Guid RoleID_pk { get; set; }
    public string RoleName { get; set; }
    public string Description { get; set; }
    public virtual ICollection<User> Users { get; set; }
  }
}
