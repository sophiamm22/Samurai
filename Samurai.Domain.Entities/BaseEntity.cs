using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Samurai.Domain.Entities
{
  public static class ObjectExtensions
  {
    public static string ToStringReflection<T>(this T @this)
    {
      return string.Join(" || ",
      new List<string>(
      from prop in @this.GetType().GetProperties(
      BindingFlags.Instance | BindingFlags.Public)
      where prop.CanRead
      select string.Format("{0}: {1}",
      prop.Name,
      prop.GetValue(@this, null))).ToArray());
    }
  }

  public abstract class BaseEntity
  {
    public virtual int Id { get; set; }

    public virtual bool IsTransient()
    {
      return Id == default(int);
    }

    public override string ToString()
    {
      return this.ToStringReflection();
    }
  }
}
