using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Modeling;

namespace Altinoren.ActiveWriter.Validation
{
    public static class ValidationHelper
    {
        internal static int GetPrimaryKeyCount(LinkedElementCollection<ModelProperty> properties)
        {
            return properties.FindAll(
                delegate(ModelProperty property)
                {
                    return (property.KeyType == KeyType.PrimaryKey);
                }
                ).Count;
        }

        internal static int GetCompositeKeyCount(LinkedElementCollection<ModelProperty> properties)
        {
            return GetCompositeKeys(properties).Count;
        }

        internal static List<ModelProperty> GetCompositeKeys(LinkedElementCollection<ModelProperty> properties)
        {
            return properties.FindAll(
                delegate(ModelProperty property)
                {
                    return (property.KeyType == KeyType.CompositeKey);
                }
                );
        }

        internal static int GetDebuggerDisplayCount(LinkedElementCollection<ModelProperty> properties)
        {
            return properties.FindAll(
                delegate(ModelProperty property)
                {
                    return (property.DebuggerDisplay);
                }
                ).Count;
        }

        internal static int GetDefaultMemberCount(LinkedElementCollection<ModelProperty> properties)
        {
            return properties.FindAll(
                delegate(ModelProperty property)
                {
                    return (property.DefaultMember);
                }
                ).Count;
        }
    }
}
