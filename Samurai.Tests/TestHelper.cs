using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using NUnit.Framework;

namespace Samurai.Tests
{
  public static partial class TestHelper
  {
    public static void PropertyValuesShouldEqual<TObject>(this TObject actual, TObject expected)
    {
      PropertyInfo[] properties = expected.GetType().GetProperties();
      foreach (PropertyInfo property in properties)
      {
        dynamic expectedValue = property.GetValue(expected, null);
        dynamic actualValue = property.GetValue(actual, null);

        if (property.GetType().GetInterfaces().Any(x => x.IsGenericType &&
          x.GetGenericTypeDefinition() == typeof(IList<>)))
        {
          AssertListsAreEquals(property, actualValue, expectedValue);
        }
        else
        {
          if (!Equals(expectedValue, actualValue))
            Assert.Fail("Property {0}.{1} does not match. Expected: {2} but was: {3}", property.DeclaringType.Name, property.Name, expectedValue, actualValue);
        }
      }
    }

    private static void AssertListsAreEquals<TListItems>(PropertyInfo property, IList<TListItems> actualList, IList<TListItems> expectedList)
    {
      if (actualList.Count != expectedList.Count)
        Assert.Fail("Property {0}.{1} does not match. Expected IList containing {2} elements but was IList containing {3} elements", property.PropertyType.Name, property.Name, expectedList.Count, actualList.Count);

      for (int i = 0; i < actualList.Count; i++)
        if (!Equals(actualList[i], expectedList[i]))
          Assert.Fail("Property {0}.{1} does not match. Expected IList with element {1} equals to {2} but was IList with element {1} equals to {3}", property.PropertyType.Name, property.Name, expectedList[i], actualList[i]);
    }
  }
}
