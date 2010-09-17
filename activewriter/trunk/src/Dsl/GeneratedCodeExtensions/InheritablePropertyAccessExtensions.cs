using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.ActiveWriter
{
    public static class InheritablePropertyAccessExtensions
    {
        public static PropertyAccess GetMatchingPropertyAccess(this InheritablePropertyAccess type)
        {
            try
            {
                return (PropertyAccess)Enum.Parse(typeof(PropertyAccess), type.ToString());
            }
            catch (ArgumentException)
            {
                throw new ArgumentOutOfRangeException("type", "InheritedRelationType can only be used as a hint to determine the derived relation's RelationType relative to the Model.");
            }
        }
    }
}
